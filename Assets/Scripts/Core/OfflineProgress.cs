using System;
using OwnADungeon.Data;
using OwnADungeon.State;

namespace OwnADungeon.Core
{
    // Direct port of src/core/offlineProgress.ts.
    public static class OfflineProgress
    {
        static readonly Random Rng = new Random();

        public static OfflineProgressSummary Simulate()
        {
            var s = SaveSystem.State;
            long nowMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            long elapsedMs = nowMs - (s.LastActiveUnixMs > 0 ? s.LastActiveUnixMs : nowMs);
            double hours = elapsedMs / (1000.0 * 60 * 60);
            if (hours < 0.25) return null;

            int raids = Math.Min(12, (int)Math.Floor(hours * 1.8));
            if (raids < 1) return null;

            int gold = 0, souls = 0, wins = 0;
            for (int i = 0; i < raids; i++)
            {
                if (Rng.NextDouble() < 0.55)
                {
                    wins++;
                    gold += 22 + s.SlotCount * 6;
                    if (Rng.NextDouble() < 0.25) souls += 1;
                }
                else gold += 8;
            }

            s.Gold += gold;
            s.Souls += souls;
            s.Stats.RaidsTotal += raids;
            s.Stats.DungeonWins += wins;
            SaveSystem.SaveState();

            return new OfflineProgressSummary { Raids = raids, Gold = gold, Souls = souls, Wins = wins, Hours = hours.ToString("F1") };
        }
    }
}
