using System.Collections.Generic;

namespace OwnADungeon.Data
{
    // Direct port of src/data/upgrades.ts.
    public static class Upgrades
    {
        public static readonly List<UpgradeDef> Defs = new List<UpgradeDef>
        {
            new UpgradeDef { Id = "spike", Label = "Spike Trap", Type = ItemKind.Trap, BaseCost = 14 },
            new UpgradeDef { Id = "poison", Label = "Poison Trap", Type = ItemKind.Trap, BaseCost = 20, RequiresUnlock = "poison" },
            new UpgradeDef { Id = "net", Label = "Net Trap", Type = ItemKind.Trap, BaseCost = 26, RequiresUnlock = "net" },
            new UpgradeDef { Id = "fire", Label = "Fire Trap", Type = ItemKind.Trap, BaseCost = 30, RequiresUnlock = "fire" },
            new UpgradeDef { Id = "frost", Label = "Frost Trap", Type = ItemKind.Trap, BaseCost = 34, RequiresUnlock = "frost" },
            new UpgradeDef { Id = "slime", Label = "Slime", Type = ItemKind.Monster, BaseCost = 14 },
            new UpgradeDef { Id = "goblin_troop", Label = "Goblin Troop", Type = ItemKind.Monster, BaseCost = 24, RequiresUnlock = "goblin_troop" },
            new UpgradeDef { Id = "goblin_shaman", Label = "Goblin Shaman", Type = ItemKind.Monster, BaseCost = 30, RequiresUnlock = "goblin_shaman" },
            new UpgradeDef { Id = "goblin_elite", Label = "Goblin Elite", Type = ItemKind.Monster, BaseCost = 38, RequiresUnlock = "goblin_elite" },
            new UpgradeDef { Id = "orc", Label = "Orc", Type = ItemKind.Monster, BaseCost = 46, RequiresUnlock = "orc" }
        };

        public static readonly List<UnlockDef> Unlocks = new List<UnlockDef>
        {
            new UnlockDef { Id = "poison", Label = "Unlock: Poison Trap", Cost = new Cost(32, 0), UnlockAtStage = 3 },
            new UnlockDef { Id = "goblin_troop", Label = "Unlock: Goblin Troop", Cost = new Cost(38, 0), UnlockAtStage = 3 },
            new UnlockDef { Id = "slot4", Label = "Dig Room 4", Cost = new Cost(90, 5), UnlockAtStage = 5 },
            new UnlockDef { Id = "net", Label = "Unlock: Net Trap", Cost = new Cost(50, 0), UnlockAtStage = 8 },
            new UnlockDef { Id = "fire", Label = "Unlock: Fire Trap", Cost = new Cost(58, 2), UnlockAtStage = 12 },
            new UnlockDef { Id = "goblin_shaman", Label = "Unlock: Goblin Shaman", Cost = new Cost(52, 2), UnlockAtStage = 14 },
            new UnlockDef { Id = "frost", Label = "Unlock: Frost Trap", Cost = new Cost(68, 3), UnlockAtStage = 17 },
            new UnlockDef { Id = "goblin_elite", Label = "Unlock: Goblin Elite", Cost = new Cost(70, 3), UnlockAtStage = 21 },
            new UnlockDef { Id = "orc", Label = "Unlock: Orc", Cost = new Cost(88, 5), UnlockAtStage = 26 },
            new UnlockDef { Id = "slot5", Label = "Dig Room 5", Cost = new Cost(150, 12), UnlockAtStage = 32 }
        };
    }
}
