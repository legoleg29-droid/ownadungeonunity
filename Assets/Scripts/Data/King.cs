using System;

namespace OwnADungeon.Data
{
    // Direct port of src/data/king.ts.
    public static class King
    {
        public static readonly KingBase Base = new KingBase
        {
            MaxHp = 48, Atk = 9, Def = 2, HpPerLevel = 14, AtkPerLevel = 2, DefPerLevel = 1
        };

        public static KingStats GetKingStats(int level = 1)
        {
            level = Math.Max(1, level);
            return new KingStats
            {
                Level = level,
                MaxHp = Base.MaxHp + (level - 1) * Base.HpPerLevel,
                Atk = Base.Atk + (level - 1) * Base.AtkPerLevel,
                Def = Base.Def + (level - 1) * Base.DefPerLevel
            };
        }

        public static readonly KingUpgradeDef Upgrade = new KingUpgradeDef
        {
            BaseGold = 35, GoldGrowth = 1.48f, SoulsEvery = 3, SoulsBase = 1
        };

        public static Cost KingUpgradeCost(int level = 1)
        {
            level = Math.Max(1, level);
            int gold = (int)Math.Round(Upgrade.BaseGold * Math.Pow(Upgrade.GoldGrowth, level - 1));
            int souls = 0;
            if (level >= 2)
            {
                souls = (level / Upgrade.SoulsEvery) * Upgrade.SoulsBase;
            }
            return new Cost(gold, souls);
        }
    }
}
