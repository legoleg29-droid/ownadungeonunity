using System.Collections.Generic;

namespace OwnADungeon.Data
{
    // Direct port of src/data/heroes.ts.
    public static class Heroes
    {
        public static readonly List<HeroArchetype> Archetypes = new List<HeroArchetype>
        {
            new HeroArchetype
            {
                Id = "warrior", Name = "Warrior", ClassName = "Warrior", Icon = "⚔", ColorHex = Theme.Bone,
                Role = "Frontline", BaseHp = 62, BaseAtk = 9, BaseDef = 4, FleeThreshold = 3,
                FearImmune = false, CanRage = false, TrapEvasion = 0.05f,
                Tags = new List<string> { "physical", "tank" },
                Strengths = "Resists spikes and brute force",
                Weaknesses = "Weak to poison and slime"
            },
            new HeroArchetype
            {
                Id = "rogue", Name = "Rogue", ClassName = "Rogue", Icon = "🗡", ColorHex = Theme.Soul,
                Role = "Skirmisher", BaseHp = 40, BaseAtk = 12, BaseDef = 1, FleeThreshold = 2,
                FearImmune = false, CanRage = false, TrapEvasion = 0.45f,
                Tags = new List<string> { "physical", "agile" },
                Strengths = "Evades traps; shreds casters",
                Weaknesses = "Struggles vs armored foes and nets"
            },
            new HeroArchetype
            {
                Id = "berserker", Name = "Berserker", ClassName = "Berserker", Icon = "🪓", ColorHex = Theme.EmberBright,
                Role = "Brawler", BaseHp = 68, BaseAtk = 11, BaseDef = 1, FleeThreshold = 999,
                FearImmune = true, CanRage = true, RageHpThreshold = 0.32f, RageAtkMultiplier = 1.45f,
                RageHealFraction = 0.12f, TrapEvasion = 0,
                Tags = new List<string> { "physical", "rage" },
                Strengths = "Fear-immune; smashes armored foes",
                Weaknesses = "Nets delay RAGE; vulnerable to DoT"
            },
            new HeroArchetype
            {
                Id = "mage", Name = "Mage", ClassName = "Mage", Icon = "✨", ColorHex = Theme.Soul,
                Role = "Caster", BaseHp = 36, BaseAtk = 13, BaseDef = 0, FleeThreshold = 2,
                FearImmune = false, CanRage = false, TrapEvasion = 0.1f, MagicAtk = true,
                Tags = new List<string> { "magic", "ranged" },
                Strengths = "Ignores some DEF; melts slime with magic",
                Weaknesses = "Fragile HP; weak to goblin bursts and orcs"
            },
            new HeroArchetype
            {
                Id = "paladin", Name = "Paladin", ClassName = "Paladin", Icon = "🛡", ColorHex = Theme.Gold,
                Role = "Support-Tank", BaseHp = 55, BaseAtk = 8, BaseDef = 3, FleeThreshold = 4,
                FearImmune = true, CanRage = false, TrapEvasion = 0.08f, Holy = true,
                Tags = new List<string> { "physical", "holy", "tank" },
                Strengths = "Outlasts orcs; resists fear",
                Weaknesses = "Slow vs agile foes; residual poison"
            }
        };

        public static readonly List<string> NamePool = new List<string>
        {
            "Sir William", "Dame Ottavia", "Korrin the Bold", "Fenwick Ash", "Brannigan",
            "Ysolde Thorn", "Garrick Vane", "Marrow the Grim", "Petra Loam", "Osric Hale",
            "Lira Quill", "Tamsin Reed", "Cedric Moss", "Vesper Grey", "Alden Crowe"
        };
    }
}
