using OwnADungeon.Combat;
using OwnADungeon.State;
using OwnADungeon.Data;

namespace OwnADungeon.Core
{
    // Direct port of src/core/resetGame.ts.
    public static class ResetGameService
    {
        public static void ResetGame()
        {
            SaveSystem.ResetState();
            HeroFactory.ClearPendingHero();
            BattleEvents.RaiseStateChanged();
            BattleEvents.RaiseToast("Game reset to the beginning", ToastType.Warning);
        }
    }
}
