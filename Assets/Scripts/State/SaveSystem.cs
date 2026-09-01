using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using OwnADungeon.Data;

namespace OwnADungeon.State
{
    // Direct port of src/state/gameState.ts. The web version persists to
    // localStorage under key "idm_state_v1"; here we persist to a JSON
    // file under Application.persistentDataPath, which is the closest
    // Unity equivalent to browser localStorage (survives app restarts,
    // per-install/per-user, not committed to source control).
    public static class SaveSystem
    {
        const string FileName = "idm_state_v1.json";

        static string SavePath => Path.Combine(Application.persistentDataPath, FileName);

        public static GameState State;

        public static GameState LoadState()
        {
            try
            {
                if (!File.Exists(SavePath))
                {
                    State = DefaultState.Create();
                    return State;
                }
                var raw = File.ReadAllText(SavePath);
                var parsed = JsonConvert.DeserializeObject<GameState>(raw);
                var next = DefaultState.Create();
                if (parsed != null)
                {
                    // Merge parsed save over a fresh default (mirrors
                    // Object.assign(structuredClone(DEFAULT_STATE), parsed)).
                    next.Gold = parsed.Gold;
                    next.Souls = parsed.Souls;
                    next.SlotCount = parsed.SlotCount != 0 ? parsed.SlotCount : next.SlotCount;
                    next.MaxSlotCount = parsed.MaxSlotCount != 0 ? parsed.MaxSlotCount : next.MaxSlotCount;
                    if (parsed.Dungeon != null && parsed.Dungeon.Count > 0) next.Dungeon = parsed.Dungeon;
                    if (parsed.Levels != null)
                        foreach (var kv in parsed.Levels) next.Levels[kv.Key] = kv.Value;
                    if (parsed.Unlocked != null)
                        foreach (var kv in parsed.Unlocked) next.Unlocked[kv.Key] = kv.Value;
                    if (parsed.Stats != null) next.Stats = parsed.Stats;
                    if (parsed.King != null && parsed.King.Level > 0) next.King = parsed.King;
                    next.Mode = parsed.Mode;
                    next.Stage = parsed.Stage;
                    next.MaxStageCleared = parsed.MaxStageCleared;
                    next.ArcadeWave = parsed.ArcadeWave;
                    next.ArcadeBest = parsed.ArcadeBest;
                    next.LastActiveUnixMs = parsed.LastActiveUnixMs;
                }

                if (next.Stage < 1) next.Stage = 1;
                if (next.Stage > Difficulty.StageMax) next.Stage = Difficulty.StageMax;
                if (next.ArcadeWave < 1) next.ArcadeWave = 1;

                State = next;
                return State;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[SaveSystem] Failed to load save, falling back to defaults: {e.Message}");
                State = DefaultState.Create();
                return State;
            }
        }

        public static void SaveState()
        {
            if (State == null) return;
            State.LastActiveUnixMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var json = JsonConvert.SerializeObject(State);
            File.WriteAllText(SavePath, json);
        }

        public static GameState ResetState()
        {
            if (File.Exists(SavePath)) File.Delete(SavePath);
            State = DefaultState.Create();
            return State;
        }
    }
}
