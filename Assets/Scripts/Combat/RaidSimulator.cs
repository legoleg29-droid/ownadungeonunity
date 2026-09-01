using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OwnADungeon.Data;
using OwnADungeon.State;
using OwnADungeon.Economy;
using OwnADungeon.Animation;

namespace OwnADungeon.Combat
{
    // Direct port of src/combat/raid.ts's runRaid(). A Unity coroutine
    // instead of an async function; every `await waitBeat(x)` becomes
    // `yield return Beat(BeatKey.X)`. Combat math, ordering, and RNG use
    // are unchanged from the web source — only the presentation calls
    // (DOM manipulation there) are replaced with BattleEvents.
    public static class RaidSimulator
    {
        static readonly System.Random Rng = new System.Random();

        static IEnumerator Beat(BeatKey key)
        {
            yield return new WaitForSeconds(BeatTiming.BeatMs(key) / 1000f);
        }

        public static IEnumerator RunRaid()
        {
            if (RuntimeState.RaidInProgress) yield break;
            RuntimeState.RaidInProgress = true;
            BattleEvents.RaiseStateChanged();

            var s = SaveSystem.State;
            var stageDiff = DifficultyResolver.GetRaidDiff();

            var hero = HeroFactory.TakePendingHero();
            BattleEvents.RaiseHeroIntro(hero);
            BattleEvents.RaiseRaidStarted();
            BattleEvents.RaisePresentRoom(-1, null); // entrance
            BattleEvents.RaiseShowHeroToken(hero);

            yield return Beat(BeatKey.EnterDungeon);
            yield return Beat(BeatKey.BetweenRooms);

            BattleEvents.RaiseShowBattleCard(hero);

            int goldReward = 0;
            int soulsReward = 0;
            bool dungeonWin = false;
            bool heroVictory = false;
            bool heroEscape = false;

            var slots = new List<DungeonSlotData>();
            for (int i = 0; i < s.SlotCount && i < s.Dungeon.Count; i++) slots.Add(s.Dungeon[i]);

            for (int i = 0; i < slots.Count; i++)
            {
                var slot = slots[i];

                BattleEvents.RaiseFlashSlot(i, SlotFlash.RaidActive);
                BattleEvents.RaisePresentRoom(i, slot);
                yield return PlayDoorEnterSequence();

                if (slot == null)
                {
                    yield return Beat(BeatKey.Resolve);
                    BattleEvents.RaiseHeroWalkToExit();
                    BattleEvents.RaiseFlashSlot(i, SlotFlash.RaidCleared);
                    yield return Beat(BeatKey.BetweenRooms);
                    continue;
                }

                var cat = Catalog.CatalogFor(slot.CatalogId, slot.Kind);
                int level = EconomyService.GetItemLevel(slot.CatalogId);

                if (slot.Kind == ItemKind.Trap && cat is TrapDef trapCat)
                {
                    HeroFactory.TriggerSurprise(hero);
                    yield return Beat(BeatKey.Threat);

                    BattleEvents.RaiseFlashSlot(i, SlotFlash.Triggered);
                    yield return Beat(BeatKey.ActionGap);

                    int baseTrap = (int)Math.Round((trapCat.BaseDamage + (level - 1) * trapCat.DmgPerLevel) * stageDiff.TrapMult);
                    float tMult = Matchups.HeroTrapMult(hero.ClassId, trapCat.Id);
                    int dmg = (int)Math.Round(baseTrap * tMult);

                    if (hero.TrapEvasion > 0 && Rng.NextDouble() < hero.TrapEvasion) dmg = 0;

                    if (dmg > 0)
                    {
                        var spec = Matchups.ApplySpecialOnTrap(hero, trapCat.Id, dmg);
                        dmg = spec.Dmg;
                        hero.Hp -= dmg;
                        HeroFactory.TriggerPain(hero);
                        BattleEvents.RaiseUpdateBattleCard(hero);
                        HeroFactory.CheckPanic(hero);
                    }

                    if (trapCat.Id == "poison" && dmg > 0)
                        hero.Status.Add(new HeroStatusEffect { Type = "poison", Rounds = trapCat.DotRounds ?? 0, Dmg = (int)Math.Round(dmg * 0.4f) });
                    if (trapCat.Id == "fire" && dmg > 0 && trapCat.BurnRounds.HasValue)
                        hero.Status.Add(new HeroStatusEffect { Type = "poison", Rounds = trapCat.BurnRounds.Value, Dmg = (int)Math.Round(dmg * 0.35f) });
                    if (trapCat.Id == "net" && dmg > 0 && trapCat.AtkReduction.HasValue)
                        hero.Atk = (int)Math.Round(hero.Atk * (1 - trapCat.AtkReduction.Value));

                    yield return Beat(BeatKey.Resolve);

                    if (hero.Hp <= 0)
                    {
                        BattleEvents.RaiseFlashSlot(i, SlotFlash.Kill);
                        HeroFactory.TriggerDeath(hero);
                        dungeonWin = true;
                        break;
                    }
                }

                if (slot.Kind == ItemKind.Monster && cat is MonsterDef monCat)
                {
                    int monHp = (int)Math.Round((monCat.BaseHp + (level - 1) * monCat.HpPerLevel) * stageDiff.MonsterHpMult);
                    int monAtk = (int)Math.Round((monCat.BaseAtk + (level - 1) * monCat.AtkPerLevel) * stageDiff.MonsterAtkMult);
                    int monDef = monCat.BaseDef;

                    BattleEvents.RaiseShowMonsterToken(monCat.Icon);
                    HeroFactory.TriggerSurprise(hero);
                    yield return Beat(BeatKey.Threat);
                    yield return Beat(BeatKey.ActionGap);

                    if (monCat.FearAura.HasValue && monCat.FearAura.Value > 0 && !hero.FearImmune)
                    {
                        if (Rng.NextDouble() < 0.22)
                        {
                            hero.Atk = Math.Max(1, (int)Math.Round(hero.Atk * 0.9f));
                            HeroFactory.TriggerFear(hero);
                        }
                    }

                    int levelGap = level - hero.Level;
                    if (!hero.FearImmune && levelGap >= hero.FleeThreshold)
                    {
                        HeroFactory.TriggerFlee(hero);
                        heroEscape = true;
                        yield return Beat(BeatKey.Resolve);
                        break;
                    }

                    int mHp = monHp;
                    while (mHp > 0 && hero.Hp > 0)
                    {
                        yield return Beat(BeatKey.CombatRound);

                        hero.Status.RemoveAll(st =>
                        {
                            if (st.Type == "poison" && st.Rounds > 0)
                            {
                                hero.Hp -= st.Dmg;
                                st.Rounds--;
                                return st.Rounds <= 0;
                            }
                            return false;
                        });
                        BattleEvents.RaiseUpdateBattleCard(hero);
                        HeroFactory.CheckPanic(hero);
                        if (hero.Hp <= 0) break;

                        if (HeroFactory.TryTriggerRage(hero))
                        {
                            BattleEvents.RaiseUpdateBattleCard(hero);
                            yield return Beat(BeatKey.ActionGap);
                        }

                        float mMult = Matchups.HeroMonsterMult(hero.ClassId, monCat.Id);
                        int raw = Math.Max(1, hero.Atk - monDef);
                        if (hero.MagicAtk) raw = Math.Max(1, hero.Atk - (int)Math.Floor(monDef * 0.4f));
                        var hit = Matchups.ApplySpecialOnMonsterHit(hero, monCat, (int)Math.Round(raw * mMult));
                        int hDmg = Math.Max(1, hit.Dmg);
                        mHp -= hDmg;

                        if (mHp <= 0)
                        {
                            BattleEvents.RaiseFlashSlot(i, SlotFlash.Cleared);
                            goldReward += 10 + level * 4;
                            break;
                        }

                        int mDmg = Math.Max(1, monAtk - hero.Def);
                        hero.Hp -= mDmg;
                        HeroFactory.TriggerPain(hero);
                        BattleEvents.RaiseUpdateBattleCard(hero);
                        HeroFactory.CheckPanic(hero);
                    }

                    yield return Beat(BeatKey.Resolve);
                    BattleEvents.RaiseHideMonsterToken();

                    if (hero.Hp <= 0)
                    {
                        BattleEvents.RaiseFlashSlot(i, SlotFlash.Kill);
                        HeroFactory.TriggerDeath(hero);
                        dungeonWin = true;
                        break;
                    }
                }

                if (slot.Kind == ItemKind.Treasure)
                {
                    yield return Beat(BeatKey.Threat);
                    yield return Beat(BeatKey.ActionGap);

                    if (hero.Hp > 0)
                    {
                        int stolen = (int)Math.Round(goldReward * 0.4f + 15);
                        goldReward = Math.Max(0, goldReward - stolen);
                        heroVictory = true;
                    }
                    yield return Beat(BeatKey.Resolve);
                }

                BattleEvents.RaiseFlashSlot(i, SlotFlash.RaidCleared);
                BattleEvents.RaiseHeroWalkToExit();
                yield return Beat(BeatKey.BetweenRooms);
            }

            if (hero.Hp > 0 && !heroEscape)
            {
                BattleEvents.RaiseFlashSlot(-2, SlotFlash.RaidActive);
                BattleEvents.RaisePresentRoom(-2, null); // throne
                yield return PlayDoorEnterSequence();
                HeroFactory.TriggerSurprise(hero);
                yield return Beat(BeatKey.Threat);

                var king = King.GetKingStats(s.King?.Level ?? 1);
                int kHp = (int)Math.Round(king.MaxHp * stageDiff.KingMult);
                int kAtk = (int)Math.Round(king.Atk * stageDiff.KingMult);
                int kDef = Math.Max(0, (int)Math.Round(king.Def * stageDiff.KingMult));

                yield return Beat(BeatKey.ActionGap);

                while (kHp > 0 && hero.Hp > 0)
                {
                    yield return Beat(BeatKey.CombatRound);

                    hero.Status.RemoveAll(st =>
                    {
                        if (st.Type == "poison" && st.Rounds > 0)
                        {
                            hero.Hp -= st.Dmg;
                            st.Rounds--;
                            return st.Rounds <= 0;
                        }
                        return false;
                    });
                    BattleEvents.RaiseUpdateBattleCard(hero);
                    HeroFactory.CheckPanic(hero);
                    if (hero.Hp <= 0) break;

                    if (HeroFactory.TryTriggerRage(hero))
                    {
                        BattleEvents.RaiseUpdateBattleCard(hero);
                        yield return Beat(BeatKey.ActionGap);
                    }

                    int hDmg = Math.Max(1, hero.Atk - kDef);
                    kHp -= hDmg;

                    if (kHp <= 0)
                    {
                        BattleEvents.RaiseFlashSlot(-2, SlotFlash.Cleared);
                        goldReward += 35 + king.Level * 10;
                        soulsReward += 1;
                        heroVictory = true;
                        break;
                    }

                    int mDmg = Math.Max(1, kAtk - hero.Def);
                    hero.Hp -= mDmg;
                    HeroFactory.TriggerPain(hero);
                    BattleEvents.RaiseUpdateBattleCard(hero);
                    HeroFactory.CheckPanic(hero);
                }

                yield return Beat(BeatKey.Resolve);

                if (hero.Hp <= 0)
                {
                    BattleEvents.RaiseFlashSlot(-2, SlotFlash.Kill);
                    HeroFactory.TriggerDeath(hero);
                    dungeonWin = true;
                }
                else
                {
                    BattleEvents.RaiseHeroWalkToExit();
                }
            }

            yield return Beat(BeatKey.Ending);
            BattleEvents.RaiseFlashSlot(-1, SlotFlash.ClearAll);
            BattleEvents.RaiseDoorOpen(false);

            HeroFactory.ClearPendingHero();

            if (hero.Hp > 0 && !heroEscape && !heroVictory) heroVictory = true;

            bool firstClear = false;

            if (dungeonWin)
            {
                goldReward += 32 + s.SlotCount * 10;
                soulsReward += 1;
                s.Stats.DungeonWins++;

                if (s.Mode == GameMode.Stage)
                {
                    firstClear = s.Stage > s.MaxStageCleared;
                    if (firstClear)
                    {
                        s.MaxStageCleared = s.Stage;
                        goldReward += stageDiff.FirstClearBonusGold;
                        soulsReward += stageDiff.FirstClearBonusSouls;
                    }
                    if (s.Stage < Difficulty.StageMax) s.Stage += 1;
                }
                else if (s.Mode == GameMode.Arcade)
                {
                    int wave = s.ArcadeWave > 0 ? s.ArcadeWave : 1;
                    if (wave > s.ArcadeBest) s.ArcadeBest = wave;
                    s.ArcadeWave = wave + 1;
                }
            }
            else if (heroEscape)
            {
                s.Stats.HeroEscapes++;
                goldReward = (int)Math.Round(goldReward * 0.35f);
            }
            else if (heroVictory)
            {
                s.Stats.HeroVictories++;
                goldReward = (int)Math.Round(goldReward * 0.45f);
            }

            if (stageDiff.RewardMult != 1)
            {
                goldReward = (int)Math.Round(goldReward * stageDiff.RewardMult);
                if (soulsReward > 0)
                    soulsReward = Math.Max(1, (int)Math.Round(soulsReward * Math.Min(2.5f, 1 + (stageDiff.RewardMult - 1) * 0.5f)));
            }

            s.Gold += goldReward;
            s.Souls += soulsReward;
            s.Stats.RaidsTotal++;

            RuntimeState.RaidInProgress = false;
            SaveSystem.SaveState();

            BattleEvents.RaiseRaidFinished(new RaidSummary
            {
                DungeonWin = dungeonWin,
                HeroVictory = heroVictory,
                HeroEscape = heroEscape,
                FirstClear = firstClear,
                GoldReward = goldReward,
                SoulsReward = soulsReward
            });
            BattleEvents.RaiseStateChanged();

            yield return new WaitForSeconds(1.4f);
            BattleEvents.RaiseHideHeroToken();
            BattleEvents.RaiseHideMonsterToken();
        }

        static IEnumerator PlayDoorEnterSequence()
        {
            BattleEvents.RaiseDoorOpen(false);
            yield return Beat(BeatKey.DoorClosed);
            BattleEvents.RaiseDoorOpen(true);
            yield return Beat(BeatKey.DoorOpen);
            yield return Beat(BeatKey.ArriveRoom);
        }
    }
}
