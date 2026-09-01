using System;

namespace OwnADungeon.Data
{
    // Direct port of src/data/difficulty.ts. Stage difficulty deliberately
    // does not scale trap/monster HP/ATK with stage number — see the
    // comment in the original source. Only the reward curve grows.
    public static class Difficulty
    {
        public const int StageMax = 50;

        public static RaidDifficulty GetStageDiff(int stage = 1)
        {
            int s = Math.Max(1, Math.Min(StageMax, stage));
            int band = s <= 5 ? 0 : s <= 20 ? 1 : s <= 35 ? 2 : 3;
            float t = (s - 1) / (float)(StageMax - 1);
            var def = Stages.GetStageDef(s);
            return new RaidDifficulty
            {
                Stage = s,
                Band = band,
                TrapMult = 1,
                MonsterHpMult = 1,
                MonsterAtkMult = 1,
                KingMult = 1,
                RewardMult = 1 + t * 0.4f,
                HeroLevelBonus = 0,
                FirstClearBonusGold = 18 + s * 3,
                FirstClearBonusSouls = s >= 10 ? 1 : 0,
                CompositionHint = def.Note
            };
        }

        public static RaidDifficulty GetArcadeDiff(int wave = 1)
        {
            int w = Math.Max(1, wave);
            float t = Math.Min(1.4f, (w - 1) * 0.04f);
            return new RaidDifficulty
            {
                Wave = w,
                TrapMult = 1 + t * 0.7f,
                MonsterHpMult = 1 + t * 0.8f,
                MonsterAtkMult = 1 + t * 0.65f,
                KingMult = 1 + t * 0.75f,
                RewardMult = 1 + t * 0.9f,
                HeroLevelBonus = (w - 1) / 6
            };
        }
    }
}
