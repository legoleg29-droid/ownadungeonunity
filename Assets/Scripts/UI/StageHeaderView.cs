using TMPro;
using UnityEngine;
using UnityEngine.UI;
using OwnADungeon.Data;
using OwnADungeon.State;
using OwnADungeon.Combat;
using OwnADungeon.Core;

namespace OwnADungeon.UI
{
    // Direct port of the "Dungeon Layout" stage header in GameApp.tsx +
    // setGameMode()/renderModeStage() in src/ui/hud.ts — the Stage/Arcade
    // mode toggle and progress label above the dungeon slots row.
    public class StageHeaderView : MonoBehaviour
    {
        Button _stageBtn, _arcadeBtn;
        Image _stageBtnBg, _arcadeBtnBg;
        TextMeshProUGUI _label;

        public static StageHeaderView Build(Transform parent)
        {
            var root = UiFactory.CreateRect("StageHeader", parent);
            var h = UiFactory.AddHorizontalLayout(root.gameObject, 10, new RectOffset(4, 4, 4, 4));
            h.childAlignment = TextAnchor.MiddleLeft;
            root.gameObject.AddComponent<LayoutElement>().preferredHeight = 40;

            var view = root.gameObject.AddComponent<StageHeaderView>();
            UiFactory.CreateText("Title", root, "Dungeon Layout", 18, UiTheme.Bone, TextAlignmentOptions.MidlineLeft, FontStyles.Bold)
                .gameObject.AddComponent<LayoutElement>().preferredWidth = 160;

            view._stageBtn = UiFactory.CreateButton("StageBtn", root, "Stage", UiTheme.WithAlpha(UiTheme.Gold, 0.25f), UiTheme.Bone, 14);
            view._stageBtnBg = view._stageBtn.GetComponent<Image>();
            view._stageBtn.gameObject.AddComponent<LayoutElement>().preferredWidth = 70;
            view._stageBtn.onClick.AddListener(() => SetMode(GameMode.Stage));

            view._arcadeBtn = UiFactory.CreateButton("ArcadeBtn", root, "Arcade", UiTheme.WithAlpha(Color.black, 0.25f), UiTheme.Bone, 14);
            view._arcadeBtnBg = view._arcadeBtn.GetComponent<Image>();
            view._arcadeBtn.gameObject.AddComponent<LayoutElement>().preferredWidth = 70;
            view._arcadeBtn.onClick.AddListener(() => SetMode(GameMode.Arcade));

            view._label = UiFactory.CreateText("Label", root, "Stage 1 / 50", 12, UiTheme.Muted);
            view._label.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1;

            BattleEvents.OnStateChanged += view.Refresh;
            view.Refresh();
            return view;
        }

        static void SetMode(GameMode mode)
        {
            if (RuntimeState.RaidInProgress)
            {
                BattleEvents.RaiseToast("Wait for the raid to finish", ToastType.Warning);
                return;
            }
            var s = SaveSystem.State;
            if (s.Mode == mode) return;
            s.Mode = mode;
            HeroFactory.ClearPendingHero();
            SaveSystem.SaveState();
            BattleEvents.RaiseStateChanged();
            BattleEvents.RaiseToast(mode == GameMode.Arcade ? "Arcade Mode" : "Stage Mode", ToastType.Info);
        }

        public void Refresh()
        {
            var s = SaveSystem.State;
            bool stage = s.Mode == GameMode.Stage;
            _stageBtnBg.color = stage ? UiTheme.WithAlpha(UiTheme.Gold, 0.25f) : UiTheme.WithAlpha(Color.black, 0.25f);
            _arcadeBtnBg.color = !stage ? UiTheme.WithAlpha(UiTheme.Gold, 0.25f) : UiTheme.WithAlpha(Color.black, 0.25f);
            _label.text = stage
                ? $"{s.Stage} / {Difficulty.StageMax} · Clear {s.MaxStageCleared}"
                : $"Wave {s.ArcadeWave} · Best {s.ArcadeBest}";
        }
    }
}
