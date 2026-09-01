using System.Collections.Generic;

namespace OwnADungeon.Data
{
    // Direct port of src/types.ts. Kept as plain C# (no MonoBehaviour) so it
    // can be shared by both game logic and UI without a scene dependency.

    public enum ItemKind { Trap, Monster, Treasure }

    public class Cost
    {
        public int Gold;
        public int Souls;

        public Cost() { }
        public Cost(int gold, int souls) { Gold = gold; Souls = souls; }
    }

    public abstract class CatalogItem
    {
        public string Id;
        public string Name;
        public string Icon;
        public ItemKind Kind;
        public List<string> Tags = new List<string>();
        public string Desc;
    }

    public class TrapDef : CatalogItem
    {
        public int BaseDamage;
        public int DmgPerLevel;
        public Cost Cost;
        public int? DotRounds;
        public float? AtkReduction;
        public int? BurnRounds;
        public float? DefReduction;
    }

    public enum MonsterType { Ranged, Brute, Tank, Resist, Ethereal }

    public class MonsterDef : CatalogItem
    {
        public MonsterType Type;
        public int BaseHp;
        public int BaseAtk;
        public int BaseDef;
        public int HpPerLevel;
        public int AtkPerLevel;
        public Cost Cost;
        public float? PhysicalResist;
        public float? FearAura;
    }

    public class TreasureDef : CatalogItem
    {
    }

    public class HeroArchetype
    {
        public string Id;
        public string Name;
        public string ClassName;
        public string Icon;
        public string ColorHex; // resolved from CSS var(--x) tokens, see Theme.cs
        public string Role;
        public int BaseHp;
        public int BaseAtk;
        public int BaseDef;
        public int FleeThreshold;
        public bool FearImmune;
        public bool CanRage;
        public float RageHpThreshold = 0.3f;
        public float RageAtkMultiplier = 1.5f;
        public float RageHealFraction = 0.15f;
        public float TrapEvasion;
        public List<string> Tags = new List<string>();
        public string Strengths;
        public string Weaknesses;
        public bool MagicAtk;
        public bool Holy;
    }

    public class KingBase
    {
        public int MaxHp;
        public int Atk;
        public int Def;
        public int HpPerLevel;
        public int AtkPerLevel;
        public int DefPerLevel;
    }

    public class KingStats
    {
        public int Level;
        public int MaxHp;
        public int Atk;
        public int Def;
    }

    public class KingUpgradeDef
    {
        public int BaseGold;
        public float GoldGrowth;
        public int SoulsEvery;
        public int SoulsBase;
    }

    public class RaidDifficulty
    {
        public float TrapMult = 1;
        public float MonsterHpMult = 1;
        public float MonsterAtkMult = 1;
        public float KingMult = 1;
        public float RewardMult = 1;
        public int HeroLevelBonus;
        public int Stage;
        public int Band;
        public int Wave;
        public int FirstClearBonusGold;
        public int FirstClearBonusSouls;
        public string CompositionHint;
    }

    public class UpgradeDef
    {
        public string Id;
        public string Label;
        public ItemKind Type; // Trap or Monster
        public int BaseCost;
        public string RequiresUnlock;
    }

    public class UnlockDef
    {
        public string Id;
        public string Label;
        public Cost Cost;
        public int UnlockAtStage;
    }

    public class StageDef
    {
        public int Stage;
        public List<string> HeroPool;
        public string Note;
    }

    public enum GameMode { Stage, Arcade }

    public class DungeonSlotData
    {
        public string CatalogId;
        public ItemKind Kind;
    }

    public class GameStats
    {
        public int RaidsTotal;
        public int DungeonWins;
        public int HeroEscapes;
        public int HeroVictories;
    }

    public class KingState
    {
        public int Level = 1;
    }

    public enum HeroVisualState { Idle, Panic, Rage, Flee, Dead }

    public class HeroStatusEffect
    {
        public string Type; // "poison" (also used for the fire burn DOT, matching the web source)
        public int Rounds;
        public int Dmg;
    }

    public class Hero
    {
        public string Name;
        public string ClassId;
        public string ClassName;
        public string Icon;
        public string ColorHex;
        public string Role;
        public int Level;
        public int MaxHp;
        public int Hp;
        public int Atk;
        public int Def;
        public int FleeThreshold;
        public bool FearImmune;
        public float TrapEvasion;
        public bool CanRage;
        public float RageHpThreshold;
        public float RageAtkMultiplier;
        public float RageHealFraction;
        public bool MagicAtk;
        public bool Holy;
        public List<string> Tags = new List<string>();
        public string Strengths;
        public string Weaknesses;
        public bool HasRaged;
        public bool NetBlocksRage;
        public List<HeroStatusEffect> Status = new List<HeroStatusEffect>();
        public HeroVisualState VisualState = HeroVisualState.Idle;
        public bool FirstStrikeUsed;
    }

    public class OfflineProgressSummary
    {
        public int Raids;
        public int Gold;
        public int Souls;
        public int Wins;
        public string Hours;
    }

    public enum ToastType { Info, Success, Warning }

    public enum ReactionKind { None, Panic, Rage, Flee, Fear, Pain, Surprise, Dead }
}
