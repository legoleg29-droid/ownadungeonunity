using System.Collections.Generic;
using OwnADungeon.State;

namespace OwnADungeon.Data
{
    // Direct port of src/data/defaultState.ts.
    public static class DefaultState
    {
        public static GameState Create()
        {
            return new GameState
            {
                Gold = 55,
                Souls = 0,
                SlotCount = 3,
                MaxSlotCount = 5,
                Dungeon = new List<DungeonSlotData> { null, null, null },
                Levels = new Dictionary<string, int>
                {
                    ["spike"] = 1, ["poison"] = 1, ["net"] = 1, ["fire"] = 1, ["frost"] = 1,
                    ["slime"] = 1, ["goblin_troop"] = 1, ["goblin_shaman"] = 1, ["goblin_elite"] = 1, ["orc"] = 1
                },
                Unlocked = new Dictionary<string, bool>
                {
                    ["spike"] = true, ["poison"] = false, ["net"] = false, ["fire"] = false, ["frost"] = false,
                    ["slime"] = true, ["goblin_troop"] = false, ["goblin_shaman"] = false, ["goblin_elite"] = false, ["orc"] = false,
                    ["slot4"] = false, ["slot5"] = false
                },
                Stats = new GameStats(),
                King = new KingState { Level = 1 },
                Mode = GameMode.Stage,
                Stage = 1,
                MaxStageCleared = 0,
                ArcadeWave = 1,
                ArcadeBest = 0,
                LastActiveUnixMs = System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };
        }
    }
}
