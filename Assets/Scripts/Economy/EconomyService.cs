using System;
using OwnADungeon.Data;
using OwnADungeon.State;

namespace OwnADungeon.Economy
{
    // Direct port of src/economy/economy.ts.
    public static class EconomyService
    {
        public static bool IsUnlocked(string id)
        {
            if (id == "spike" || id == "slime" || id == "treasure") return true;
            var s = SaveSystem.State;
            return s.Unlocked.TryGetValue(id, out var v) && v;
        }

        public static int GetItemLevel(string catalogId)
        {
            var s = SaveSystem.State;
            return s.Levels.TryGetValue(catalogId, out var lvl) ? lvl : 1;
        }

        public static bool Affordable(Cost cost)
        {
            var s = SaveSystem.State;
            return s.Gold >= cost.Gold && s.Souls >= cost.Souls;
        }

        public static void Spend(Cost cost)
        {
            var s = SaveSystem.State;
            s.Gold -= cost.Gold;
            s.Souls -= cost.Souls;
        }

        public static string CostLabel(Cost cost)
        {
            var parts = new System.Collections.Generic.List<string>();
            if (cost.Gold != 0) parts.Add(cost.Gold + "g");
            if (cost.Souls != 0) parts.Add(cost.Souls + "s");
            return parts.Count > 0 ? string.Join(" + ", parts) : "Gratis";
        }

        public static int UpgradeCost(int baseCost, int level)
        {
            return (int)Math.Round(baseCost * Math.Pow(1.5, level - 1));
        }

        public static bool TryUpgradeKing()
        {
            var s = SaveSystem.State;
            if (s.King == null) s.King = new KingState { Level = 1 };
            int level = s.King.Level > 0 ? s.King.Level : 1;
            var cost = King.KingUpgradeCost(level);
            if (!Affordable(cost)) return false;
            Spend(cost);
            s.King.Level = level + 1;
            SaveSystem.SaveState();
            return true;
        }
    }
}
