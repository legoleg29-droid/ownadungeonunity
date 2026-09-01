using System.Collections.Generic;
using OwnADungeon.Data;

namespace OwnADungeon.State
{
    // Direct port of the GameState shape in src/types.ts. Plain C# (JSON
    // serialized via Newtonsoft.Json, which — unlike Unity's built-in
    // JsonUtility — supports Dictionary<TKey,TValue> directly, matching the
    // Record<string, T> shapes used throughout the original TS state).
    public class GameState
    {
        public int Gold;
        public int Souls;
        public int SlotCount;
        public int MaxSlotCount;
        public List<DungeonSlotData> Dungeon = new List<DungeonSlotData>();
        public Dictionary<string, int> Levels = new Dictionary<string, int>();
        public Dictionary<string, bool> Unlocked = new Dictionary<string, bool>();
        public GameStats Stats = new GameStats();
        public KingState King = new KingState();
        public GameMode Mode = GameMode.Stage;
        public int Stage = 1;
        public int MaxStageCleared;
        public int ArcadeWave = 1;
        public int ArcadeBest;
        public long LastActiveUnixMs;
    }
}
