using System.Collections.Generic;

namespace OwnADungeon.Data
{
    // Direct port of src/data/traps.ts + src/data/monsters.ts + src/data/catalog.ts.
    // Numbers are copied verbatim from the web source so combat balance is
    // unchanged. Icons stay as the original emoji strings (see the UI layer
    // for how they're displayed) rather than being remapped to the unused
    // monster sprite sheets under public/assets/monsters/ — the live web
    // game never wired those sprites in, so this migration doesn't either.
    public static class Catalog
    {
        public static readonly Dictionary<string, TrapDef> Traps = new Dictionary<string, TrapDef>
        {
            ["spike"] = new TrapDef
            {
                Id = "spike", Name = "Spike Trap", Icon = "🗡", Kind = ItemKind.Trap,
                Tags = new List<string> { "physical", "instant" },
                Desc = "Damage fisik instan. Lemah vs armor tebal.",
                BaseDamage = 14, DmgPerLevel = 5, Cost = new Cost(0, 0)
            },
            ["poison"] = new TrapDef
            {
                Id = "poison", Name = "Poison Trap", Icon = "☠", Kind = ItemKind.Trap,
                Tags = new List<string> { "dot", "nature" },
                Desc = "DOT tiap giliran. Menyiksa high-HP.",
                BaseDamage = 6, DmgPerLevel = 3, DotRounds = 3, Cost = new Cost(32, 0)
            },
            ["net"] = new TrapDef
            {
                Id = "net", Name = "Net Trap", Icon = "🕸", Kind = ItemKind.Trap,
                Tags = new List<string> { "control" },
                Desc = "Turunkan ATK. Menunda RAGE Berserker.",
                BaseDamage = 4, DmgPerLevel = 2, AtkReduction = 0.28f, Cost = new Cost(48, 0)
            },
            ["fire"] = new TrapDef
            {
                Id = "fire", Name = "Fire Trap", Icon = "🔥", Kind = ItemKind.Trap,
                Tags = new List<string> { "fire", "dot" },
                Desc = "Burn instan + sisa panas. Kuat vs slime.",
                BaseDamage = 11, DmgPerLevel = 4, BurnRounds = 2, Cost = new Cost(60, 2)
            },
            ["frost"] = new TrapDef
            {
                Id = "frost", Name = "Frost Trap", Icon = "❄", Kind = ItemKind.Trap,
                Tags = new List<string> { "control", "cold" },
                Desc = "Kurangi DEF hero sementara. Soften tank.",
                BaseDamage = 5, DmgPerLevel = 2, DefReduction = 0.35f, Cost = new Cost(70, 3)
            }
        };

        public static readonly Dictionary<string, MonsterDef> Monsters = new Dictionary<string, MonsterDef>
        {
            ["slime"] = new MonsterDef
            {
                Id = "slime", Name = "Slime", Icon = "🟢", Kind = ItemKind.Monster, Type = MonsterType.Resist,
                Tags = new List<string> { "acid", "resist" },
                Desc = "Resists physical hits. Weak to magic and fire. Always available.",
                BaseHp = 22, BaseAtk = 6, BaseDef = 2, HpPerLevel = 5, AtkPerLevel = 1,
                PhysicalResist = 0.3f, Cost = new Cost(0, 0)
            },
            ["goblin_troop"] = new MonsterDef
            {
                Id = "goblin_troop", Name = "Goblin Troop", Icon = "👹", Kind = ItemKind.Monster, Type = MonsterType.Brute,
                Tags = new List<string> { "physical", "burst" },
                Desc = "Fast burst damage, thin defense.",
                BaseHp = 28, BaseAtk = 12, BaseDef = 1, HpPerLevel = 6, AtkPerLevel = 2,
                Cost = new Cost(40, 0)
            },
            ["goblin_shaman"] = new MonsterDef
            {
                Id = "goblin_shaman", Name = "Goblin Shaman", Icon = "🪄", Kind = ItemKind.Monster, Type = MonsterType.Ranged,
                Tags = new List<string> { "magic", "ranged" },
                Desc = "Ranged caster chip damage. Frail up close.",
                BaseHp = 32, BaseAtk = 11, BaseDef = 2, HpPerLevel = 7, AtkPerLevel = 2,
                Cost = new Cost(55, 2)
            },
            ["goblin_elite"] = new MonsterDef
            {
                Id = "goblin_elite", Name = "Goblin Elite", Icon = "🛡️", Kind = ItemKind.Monster, Type = MonsterType.Tank,
                Tags = new List<string> { "physical", "tank", "armored" },
                Desc = "Armored bruiser. High HP and DEF.",
                BaseHp = 46, BaseAtk = 14, BaseDef = 4, HpPerLevel = 10, AtkPerLevel = 2,
                Cost = new Cost(70, 3)
            },
            ["orc"] = new MonsterDef
            {
                Id = "orc", Name = "Orc", Icon = "👺", Kind = ItemKind.Monster, Type = MonsterType.Brute,
                Tags = new List<string> { "physical", "brute", "heavy" },
                Desc = "Endgame heavy hitter. Massive HP and ATK.",
                BaseHp = 60, BaseAtk = 17, BaseDef = 5, HpPerLevel = 13, AtkPerLevel = 3,
                Cost = new Cost(90, 5)
            }
        };

        public static readonly TreasureDef Treasure = new TreasureDef
        {
            Id = "treasure", Name = "Treasure Vault", Icon = "💰", Kind = ItemKind.Treasure,
            Desc = "If the hero reaches this room alive, they steal some of your reward."
        };

        public static CatalogItem CatalogFor(string catalogId, ItemKind kind)
        {
            switch (kind)
            {
                case ItemKind.Trap:
                    return Traps.TryGetValue(catalogId, out var t) ? t : null;
                case ItemKind.Monster:
                    return Monsters.TryGetValue(catalogId, out var m) ? m : null;
                case ItemKind.Treasure:
                    return Treasure;
                default:
                    return null;
            }
        }
    }
}
