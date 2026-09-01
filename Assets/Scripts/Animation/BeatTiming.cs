using System;
using System.Collections.Generic;

namespace OwnADungeon.Animation
{
    public enum BeatKey
    {
        EnterDungeon, DoorClosed, DoorOpen, ArriveRoom, Threat,
        ActionGap, CombatRound, Resolve, BetweenRooms, Ending
    }

    // Direct port of src/animation/beatTiming.ts. beatMs() returns a
    // jittered duration in milliseconds; RaidSimulator waits that many
    // milliseconds (in real seconds via a coroutine) between raid beats.
    public static class BeatTiming
    {
        const float Jitter = 0.3f;

        static readonly Dictionary<BeatKey, int> StageBeatMs = new Dictionary<BeatKey, int>
        {
            [BeatKey.EnterDungeon] = 850,
            [BeatKey.DoorClosed] = 500,
            [BeatKey.DoorOpen] = 600,
            [BeatKey.ArriveRoom] = 750,
            [BeatKey.Threat] = 800,
            [BeatKey.ActionGap] = 700,
            [BeatKey.CombatRound] = 950,
            [BeatKey.Resolve] = 850,
            [BeatKey.BetweenRooms] = 650,
            [BeatKey.Ending] = 1100
        };

        static readonly System.Random Rng = new System.Random();

        public static int BeatMs(BeatKey key)
        {
            int baseMs = StageBeatMs.TryGetValue(key, out var v) ? v : 500;
            double factor = 1 + (Rng.NextDouble() * 2 - 1) * Jitter;
            return Math.Max(120, (int)Math.Round(baseMs * factor));
        }
    }
}
