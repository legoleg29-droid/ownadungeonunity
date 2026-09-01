using System;
using OwnADungeon.Data;

namespace OwnADungeon.Combat
{
    public enum SlotFlash { Triggered, Cleared, Kill, RaidActive, RaidCleared, ClearAll }

    // Replaces the direct DOM manipulation the web version does from
    // src/combat/hero.ts and src/combat/raid.ts (setBattleReaction,
    // updateBattleCard, flashSlot, document.getElementById(...), etc.)
    // with plain C# events. UI code (Assets/Scripts/UI) subscribes to these
    // to drive the on-screen presentation; combat logic itself never
    // touches a UI object directly, mirroring the original module split.
    public static class BattleEvents
    {
        public static event Action<string, ReactionKind> OnBattleReaction;
        public static event Action<Hero> OnHeroVisualSync;
        public static event Action<string, ToastType> OnToast;

        public static event Action OnRaidStarted;
        public static event Action<Hero> OnHeroIntro;
        public static event Action<int, DungeonSlotData> OnPresentRoom; // index == -1 => entrance, -2 => throne
        public static event Action<bool> OnDoorOpen;
        public static event Action<Hero> OnShowHeroToken;
        public static event Action OnHideHeroToken;
        public static event Action<string> OnShowMonsterToken; // icon
        public static event Action OnHideMonsterToken;
        public static event Action OnHeroWalkToExit;
        public static event Action<Hero> OnShowBattleCard;
        public static event Action<Hero> OnUpdateBattleCard;
        public static event Action<int, SlotFlash> OnFlashSlot; // slot index, or -2 for throne
        public static event Action<RaidSummary> OnRaidFinished;
        public static event Action OnStateChanged; // coarse "re-render everything" signal (renderBus.ts equivalent)

        public static void RaiseReaction(string text, ReactionKind kind) => OnBattleReaction?.Invoke(text, kind);
        public static void RaiseHeroVisualSync(Hero h) => OnHeroVisualSync?.Invoke(h);
        public static void RaiseToast(string text, ToastType type) => OnToast?.Invoke(text, type);
        public static void RaiseRaidStarted() => OnRaidStarted?.Invoke();
        public static void RaiseHeroIntro(Hero h) => OnHeroIntro?.Invoke(h);
        public static void RaisePresentRoom(int index, DungeonSlotData slot) => OnPresentRoom?.Invoke(index, slot);
        public static void RaiseDoorOpen(bool open) => OnDoorOpen?.Invoke(open);
        public static void RaiseShowHeroToken(Hero h) => OnShowHeroToken?.Invoke(h);
        public static void RaiseHideHeroToken() => OnHideHeroToken?.Invoke();
        public static void RaiseShowMonsterToken(string icon) => OnShowMonsterToken?.Invoke(icon);
        public static void RaiseHideMonsterToken() => OnHideMonsterToken?.Invoke();
        public static void RaiseHeroWalkToExit() => OnHeroWalkToExit?.Invoke();
        public static void RaiseShowBattleCard(Hero h) => OnShowBattleCard?.Invoke(h);
        public static void RaiseUpdateBattleCard(Hero h) => OnUpdateBattleCard?.Invoke(h);
        public static void RaiseFlashSlot(int index, SlotFlash flash) => OnFlashSlot?.Invoke(index, flash);
        public static void RaiseRaidFinished(RaidSummary s) => OnRaidFinished?.Invoke(s);
        public static void RaiseStateChanged() => OnStateChanged?.Invoke();
    }

    public class RaidSummary
    {
        public bool DungeonWin;
        public bool HeroVictory;
        public bool HeroEscape;
        public bool FirstClear;
        public int GoldReward;
        public int SoulsReward;
    }
}
