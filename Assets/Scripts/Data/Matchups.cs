using System.Collections.Generic;

namespace OwnADungeon.Data
{
    public enum MatchupLabel { Strong, Weak, Neutral }

    public class SpecialTrapResult { public int Dmg; public string Special; }
    public class SpecialMonsterResult { public int Dmg; public string Note; }

    // Direct port of src/data/matchups.ts.
    public static class Matchups
    {
        const float Strong = 1.25f;
        const float Weak = 0.8f;

        public static readonly Dictionary<string, Dictionary<string, float>> HeroVsMonster =
            new Dictionary<string, Dictionary<string, float>>
            {
                ["warrior"] = new Dictionary<string, float> { ["slime"] = Weak, ["goblin_troop"] = 1.1f, ["goblin_shaman"] = 1.05f, ["goblin_elite"] = 1.15f, ["orc"] = 0.95f },
                ["rogue"] = new Dictionary<string, float> { ["slime"] = 0.85f, ["goblin_troop"] = 0.95f, ["goblin_shaman"] = Strong, ["goblin_elite"] = Weak, ["orc"] = 0.9f },
                ["berserker"] = new Dictionary<string, float> { ["slime"] = 0.9f, ["goblin_troop"] = 1.1f, ["goblin_shaman"] = 1.05f, ["goblin_elite"] = Strong, ["orc"] = 1.0f },
                ["mage"] = new Dictionary<string, float> { ["slime"] = Strong, ["goblin_troop"] = Weak, ["goblin_shaman"] = 1.1f, ["goblin_elite"] = 1.05f, ["orc"] = Weak },
                ["paladin"] = new Dictionary<string, float> { ["slime"] = 0.9f, ["goblin_troop"] = 0.95f, ["goblin_shaman"] = 1.1f, ["goblin_elite"] = 1.1f, ["orc"] = Strong }
            };

        public static readonly Dictionary<string, Dictionary<string, float>> HeroVsTrap =
            new Dictionary<string, Dictionary<string, float>>
            {
                ["warrior"] = new Dictionary<string, float> { ["spike"] = 0.72f, ["poison"] = 1.28f, ["net"] = 1.0f, ["fire"] = 1.05f, ["frost"] = 0.95f },
                ["rogue"] = new Dictionary<string, float> { ["spike"] = 0.85f, ["poison"] = 1.05f, ["net"] = 1.22f, ["fire"] = 1.0f, ["frost"] = 1.05f },
                ["berserker"] = new Dictionary<string, float> { ["spike"] = 1.08f, ["poison"] = 1.2f, ["net"] = 1.15f, ["fire"] = 1.1f, ["frost"] = 1.0f },
                ["mage"] = new Dictionary<string, float> { ["spike"] = 1.3f, ["poison"] = 1.0f, ["net"] = 1.05f, ["fire"] = 0.9f, ["frost"] = 0.88f },
                ["paladin"] = new Dictionary<string, float> { ["spike"] = 0.9f, ["poison"] = 1.22f, ["net"] = 0.95f, ["fire"] = 1.0f, ["frost"] = 0.92f }
            };

        public static float HeroMonsterMult(string heroClassId, string monsterId)
        {
            if (!HeroVsMonster.TryGetValue(heroClassId, out var row)) return 1f;
            return row.TryGetValue(monsterId, out var m) ? m : 1f;
        }

        public static float HeroTrapMult(string heroClassId, string trapId)
        {
            if (!HeroVsTrap.TryGetValue(heroClassId, out var row)) return 1f;
            return row.TryGetValue(trapId, out var m) ? m : 1f;
        }

        public static MatchupLabel GetMatchupLabel(float mult)
        {
            if (mult >= 1.2f) return MatchupLabel.Strong;
            if (mult <= 0.85f) return MatchupLabel.Weak;
            return MatchupLabel.Neutral;
        }

        public static SpecialTrapResult ApplySpecialOnTrap(Hero hero, string trapId, int baseDmg)
        {
            string special = null;
            int dmg = baseDmg;
            if (trapId == "net" && hero.ClassId == "berserker")
            {
                hero.NetBlocksRage = true;
                special = "net_blocks_rage";
            }
            if (trapId == "frost")
            {
                hero.Def = System.Math.Max(0, (int)System.Math.Round(hero.Def * (1 - 0.35f)));
                special = "frost_def";
            }
            if (trapId == "fire" && hero.ClassId == "mage")
            {
                dmg = (int)System.Math.Round(dmg * 0.85f);
                special = "mage_fire_resist";
            }
            return new SpecialTrapResult { Dmg = dmg, Special = special };
        }

        public static SpecialMonsterResult ApplySpecialOnMonsterHit(Hero hero, MonsterDef monster, int heroDmg)
        {
            int dmg = heroDmg;
            string note = null;
            if (monster.PhysicalResist.HasValue && monster.PhysicalResist.Value > 0 && !hero.MagicAtk && !hero.Holy)
            {
                dmg = (int)System.Math.Round(dmg * (1 - monster.PhysicalResist.Value));
                note = "physical_resist";
            }
            // Magic bypasses a resist-type monster's physical mitigation.
            if (hero.MagicAtk && monster.Type == MonsterType.Resist)
            {
                dmg = (int)System.Math.Round(dmg * 1.08f);
                note = "magic_bonus";
            }
            // Holy damage bonus vs undead (tag-driven; current roster has no undead entries yet).
            if (hero.Holy && monster.Tags.Contains("undead"))
            {
                dmg = (int)System.Math.Round(dmg * 1.12f);
                note = "holy_bonus";
            }
            if (hero.ClassId == "rogue" && monster.Type == MonsterType.Ranged && !hero.FirstStrikeUsed)
            {
                dmg = (int)System.Math.Round(dmg * 1.2f);
                hero.FirstStrikeUsed = true;
                note = "first_strike";
            }
            return new SpecialMonsterResult { Dmg = dmg, Note = note };
        }
    }
}
