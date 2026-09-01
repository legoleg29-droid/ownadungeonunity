using OwnADungeon.Data;

namespace OwnADungeon.State
{
    public class SelectedPaletteItem
    {
        public string Id;
        public ItemKind Kind;
    }

    // Direct port of src/state/runtimeState.ts — ephemeral, non-persisted
    // session state shared across UI/combat code.
    public static class RuntimeState
    {
        public static SelectedPaletteItem SelectedPaletteItem;
        public static bool RaidInProgress;
        public static Hero PendingHero;
    }
}
