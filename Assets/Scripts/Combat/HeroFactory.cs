using System;
using System.Collections.Generic;
using System.Linq;
using OwnADungeon.Data;
using OwnADungeon.State;
using OwnADungeon.Economy;

namespace OwnADungeon.Combat
{
    // Direct port of src/combat/hero.ts.
    public static class HeroFactory
    {
        static readonly Random Rng = new Random();

        static List<HeroArchetype> EligibleArchetypes()
        {
            var s = SaveSystem.State;
            if (s.Mode != GameMode.Stage) return Heroes.Archetypes;
            var pool = Stages.GetStageDef(s.Stage).HeroPool;
            var filtered = Heroes.Archetypes.Where(a => pool.Contains(a.Id)).ToList();
            return filtered.Count > 0 ? filtered : Heroes.Archetypes;
        }

        public static Hero BuildHero()
        {
            var s = SaveSystem.State;
            var roster = EligibleArchetypes();
            var arch = roster[Rng.Next(roster.Count)];
            var name = Heroes.NamePool[Rng.Next(Heroes.NamePool.Count)];

            var unlockedIds = s.Levels.Keys.Where(id => EconomyService.IsUnlocked(id) || id == "spike" || id == "slime").ToList();
            var vals = unlockedIds.Select(id => s.Levels.TryGetValue(id, out var l) ? l : 1).ToList();
            int avgLevel = Math.Max(1, (int)Math.Round(vals.Count > 0 ? vals.Average() : 1));
            int stageBonus = DifficultyResolver.GetRaidDiff().HeroLevelBonus;
            int level = Math.Max(1, avgLevel + Rng.Next(3) - 1 + stageBonus);

            int hp = (int)Math.Round(arch.BaseHp + (level - 1) * 8.0);
            int atk = (int)Math.Round(arch.BaseAtk + (level - 1) * 1.5);
            int def = (int)Math.Round(arch.BaseDef + (level - 1) * 0.4);

            return new Hero
            {
                Name = name,
                ClassId = arch.Id,
                ClassName = arch.ClassName,
                Icon = arch.Icon,
                ColorHex = arch.ColorHex,
                Role = arch.Role,
                Level = level,
                MaxHp = hp,
                Hp = hp,
                Atk = atk,
                Def = def,
                FleeThreshold = arch.FleeThreshold,
                FearImmune = arch.FearImmune,
                TrapEvasion = arch.TrapEvasion,
                CanRage = arch.CanRage,
                RageHpThreshold = arch.RageHpThreshold,
                RageAtkMultiplier = arch.RageAtkMultiplier,
                RageHealFraction = arch.RageHealFraction,
                MagicAtk = arch.MagicAtk,
                Holy = arch.Holy,
                Tags = new List<string>(arch.Tags),
                Strengths = arch.Strengths,
                Weaknesses = arch.Weaknesses,
                HasRaged = false,
                NetBlocksRage = false,
                Status = new List<HeroStatusEffect>(),
                VisualState = HeroVisualState.Idle
            };
        }

        public static void ClearPendingHero() => RuntimeState.PendingHero = null;

        public static Hero EnsurePendingHero()
        {
            if (RuntimeState.PendingHero == null) RuntimeState.PendingHero = BuildHero();
            return RuntimeState.PendingHero;
        }

        public static Hero TakePendingHero()
        {
            var hero = EnsurePendingHero();
            RuntimeState.PendingHero = null;
            return hero;
        }

        static void SetHeroReaction(Hero hero, ReactionKind kind, string text)
        {
            BattleEvents.RaiseReaction(text, kind);
            BattleEvents.RaiseHeroVisualSync(hero);
        }

        public static void CheckPanic(Hero hero)
        {
            if (hero == null || hero.Hp <= 0) return;
            if (hero.VisualState == HeroVisualState.Dead || hero.VisualState == HeroVisualState.Flee) return;
            if (hero.Hp / (float)hero.MaxHp <= 0.35f && hero.VisualState != HeroVisualState.Rage)
            {
                hero.VisualState = HeroVisualState.Panic;
                SetHeroReaction(hero, ReactionKind.Panic, "PANIK");
            }
        }

        public static bool TryTriggerRage(Hero hero)
        {
            if (hero == null || !hero.CanRage || hero.HasRaged) return false;
            if (hero.NetBlocksRage) return false;
            if (hero.Hp / (float)hero.MaxHp > hero.RageHpThreshold) return false;
            hero.HasRaged = true;
            hero.VisualState = HeroVisualState.Rage;
            hero.Atk = (int)Math.Round(hero.Atk * hero.RageAtkMultiplier);
            hero.Hp = Math.Min(hero.MaxHp, hero.Hp + (int)Math.Round(hero.MaxHp * hero.RageHealFraction));
            SetHeroReaction(hero, ReactionKind.Rage, "RAGE");
            return true;
        }

        public static void TriggerFlee(Hero hero)
        {
            if (hero == null) return;
            hero.VisualState = HeroVisualState.Flee;
            SetHeroReaction(hero, ReactionKind.Flee, "KABUR");
        }

        public static void TriggerDeath(Hero hero)
        {
            if (hero == null) return;
            hero.Hp = 0;
            hero.VisualState = HeroVisualState.Dead;
            SetHeroReaction(hero, ReactionKind.Dead, "");
        }

        public static void TriggerPain(Hero hero)
        {
            if (hero == null || hero.Hp <= 0) return;
            if (hero.VisualState == HeroVisualState.Dead || hero.VisualState == HeroVisualState.Flee) return;
            BattleEvents.RaiseReaction("SAKIT!", ReactionKind.Pain);
        }

        public static void TriggerSurprise(Hero hero)
        {
            if (hero == null || hero.Hp <= 0) return;
            if (hero.VisualState == HeroVisualState.Dead || hero.VisualState == HeroVisualState.Flee) return;
            BattleEvents.RaiseReaction("TERKEJUT", ReactionKind.Surprise);
        }

        public static void TriggerFear(Hero hero)
        {
            if (hero == null || hero.Hp <= 0 || hero.FearImmune) return;
            if (hero.VisualState == HeroVisualState.Dead || hero.VisualState == HeroVisualState.Flee) return;
            BattleEvents.RaiseReaction("TAKUT", ReactionKind.Fear);
        }
    }
}
