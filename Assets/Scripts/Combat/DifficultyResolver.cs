using OwnADungeon.Data;
using OwnADungeon.State;

namespace OwnADungeon.Combat
{
    // Direct port of src/combat/difficultyResolver.ts.
    public static class DifficultyResolver
    {
        public static RaidDifficulty GetRaidDiff()
        {
            var s = SaveSystem.State;
            if (s.Mode == GameMode.Arcade) return Difficulty.GetArcadeDiff(s.ArcadeWave);
            return Difficulty.GetStageDiff(s.Stage);
        }
    }
}
