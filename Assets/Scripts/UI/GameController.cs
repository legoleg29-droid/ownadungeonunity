using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using OwnADungeon.Data;
using OwnADungeon.State;
using OwnADungeon.Combat;
using OwnADungeon.Core;

namespace OwnADungeon.UI
{
    // Top-level orchestrator — the Unity equivalent of game-client.ts's
    // startGame(): loads the save, simulates offline progress, builds the
    // entire UI tree at runtime (see UiFactory's header comment for why),
    // and wires every button. Attach this to a single empty GameObject in
    // the scene ("GameBootstrap"); everything else is created from code.
    public class GameController : MonoBehaviour
    {
        Button _raidButton;
        TextMeshProUGUI _raidButtonLabel;

        // Self-bootstrapping on purpose: the whole UI tree is built from
        // code (see UiFactory's header comment), so nothing in the scene
        // needs to reference this script directly. That sidesteps having
        // to hand-author a scene file that names this MonoBehaviour by its
        // Unity-assigned script GUID — a GUID this environment has no
        // Editor available to generate or verify. Main.unity can stay a
        // plain empty scene (camera + light only).
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Bootstrap()
        {
            var go = new GameObject("GameBootstrap");
            go.AddComponent<GameController>();
            DontDestroyOnLoad(go);
        }

        void Start()
        {
            SaveSystem.LoadState();
            var offlineSummary = OfflineProgress.Simulate();

            var canvasGo = new GameObject("Canvas", typeof(RectTransform));
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            // Reference resolution matches the web build's mobile-first
            // portrait frame (max-width: 480 CSS px scaled ~2.25x for a
            // typical device pixel ratio).
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            canvasGo.AddComponent<GraphicRaycaster>();

            if (FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                var es = new GameObject("EventSystem");
                es.AddComponent<UnityEngine.EventSystems.EventSystem>();
                es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }

            // Root "app" frame — plain background only. FAB, overlays, and
            // modals are anchored/stretched directly under this (mirroring
            // how position:fixed/absolute overlays sit inside .app in the
            // web version), so it deliberately has NO layout group — a
            // LayoutGroup would fight their anchors every layout pass.
            var app = UiFactory.CreateImage("App", canvas.transform, UiTheme.Bg);
            UiFactory.Stretch(app.rectTransform);

            // The actual vertical flow (HUD, then the scrolling main
            // column) lives in its own child so it can use a
            // VerticalLayoutGroup without affecting the overlay/FAB
            // children of "app".
            var content = UiFactory.CreateRect("Content", app.transform);
            UiFactory.Stretch(content);
            // Leave room at the bottom for the bottom nav + play FAB,
            // which are pinned there independently of this flow (mirrors
            // .app's padding-bottom in app/styles/layout.css).
            content.offsetMin = new Vector2(content.offsetMin.x, 150);
            var appLayout = UiFactory.AddVerticalLayout(content.gameObject, 8, new RectOffset(10, 10, 8, 8));
            appLayout.childForceExpandHeight = false;

            HudView.Build(content);

            // Main scrolling column: stage header, dungeon slots, room
            // preview/battle card, room stage.
            var main = UiFactory.CreateRect("Main", content);
            var mainLayout = UiFactory.AddVerticalLayout(main.gameObject, 8, null, true, true);
            main.gameObject.AddComponent<LayoutElement>().flexibleHeight = 1;

            StageHeaderView.Build(main);
            DungeonSlotsView.Build(main);

            var previewPanel = UiFactory.CreateImage("RoomPreview", main, UiTheme.WithAlpha(Color.black, 0.25f));
            var stagePanelHolder = UiFactory.CreateRect("RoomStageHolder", main);
            UiFactory.AddVerticalLayout(stagePanelHolder.gameObject, 0, null, true, true);

            var raidStatus = UiFactory.CreateText("RaidStatus", main, "", 12, UiTheme.Muted, TextAlignmentOptions.Center);
            raidStatus.gameObject.AddComponent<LayoutElement>().preferredHeight = 18;

            BattleStageView.Build(previewPanel.rectTransform, stagePanelHolder);

            // Play FAB
            var fab = UiFactory.CreateButton("PlayFab", app.transform, "▶ RAID", UiTheme.Ember, UiTheme.Bone, 20);
            UiFactory.Anchor(fab.GetComponent<RectTransform>(), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f));
            fab.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 110);
            fab.GetComponent<RectTransform>().sizeDelta = new Vector2(140, 64);
            var fabOutline = fab.gameObject.AddComponent<Outline>();
            fabOutline.effectColor = UiTheme.EmberBright;
            _raidButton = fab;
            _raidButtonLabel = fab.GetComponentInChildren<TextMeshProUGUI>();
            fab.onClick.AddListener(() => StartCoroutine(RaidSimulator.RunRaid()));

            // Overlays
            var palette = PaletteView.Build(app.transform, null);
            var upgrades = UpgradesView.Build(app.transform);
            var stats = StatsView.Build(app.transform);

            var resetModal = ModalPanel.Build(app.transform, "Reset Game?");
            UiFactory.CreateText("ResetBody", resetModal.Body,
                "All progress will be wiped: gold, souls, upgrades, unlocks, dungeon layout, and stats. This cannot be undone.",
                14, UiTheme.Bone);
            var resetActions = UiFactory.CreateRect("Actions", resetModal.Body);
            UiFactory.AddHorizontalLayout(resetActions.gameObject, 10);
            resetActions.gameObject.AddComponent<LayoutElement>().preferredHeight = 44;
            var cancelBtn = UiFactory.CreateButton("Cancel", resetActions, "Cancel", UiTheme.WithAlpha(Color.white, 0.08f), UiTheme.Bone, 16);
            cancelBtn.gameObject.AddComponent<LayoutElement>().preferredWidth = 120;
            cancelBtn.onClick.AddListener(() => resetModal.Hide());
            var confirmBtn = UiFactory.CreateButton("Confirm", resetActions, "Yes, Reset", UiTheme.WithAlpha(UiTheme.Danger, 0.4f), UiTheme.Bone, 16);
            confirmBtn.gameObject.AddComponent<LayoutElement>().preferredWidth = 150;
            confirmBtn.onClick.AddListener(() =>
            {
                resetModal.Hide();
                ResetGameService.ResetGame();
            });

            var settings = SettingsView.Build(app.transform, resetModal);
            var settingsOverlay = settings.GetComponent<OverlayPanel>();

            BottomNavView.Build(app.transform,
                palette.GetComponent<OverlayPanel>(),
                upgrades.GetComponent<OverlayPanel>(),
                stats.GetComponent<OverlayPanel>(),
                settingsOverlay);

            var toasts = gameObject.AddComponent<ToastManager>();
            toasts.Build(canvas.transform);

            BattleEvents.OnStateChanged += () =>
            {
                _raidButton.interactable = !RuntimeState.RaidInProgress;
            };

            BattleEvents.RaiseStateChanged();

            if (offlineSummary != null)
            {
                var offlineModal = ModalPanel.Build(app.transform, "While You Were Away...");
                UiFactory.CreateText("L1", offlineModal.Body, $"You were away for ~{offlineSummary.Hours} hours.", 14, UiTheme.Bone);
                UiFactory.CreateText("L2", offlineModal.Body, $"Offline simulation: {offlineSummary.Raids} raids.", 14, UiTheme.Bone);
                UiFactory.CreateText("L3", offlineModal.Body, $"Dungeon wins: {offlineSummary.Wins}", 14, UiTheme.Bone);
                UiFactory.CreateText("L4", offlineModal.Body, $"Gold earned: +{offlineSummary.Gold}", 14, UiTheme.Gold);
                UiFactory.CreateText("L5", offlineModal.Body, $"Souls earned: +{offlineSummary.Souls}", 14, UiTheme.Soul);
                var closeBtn = UiFactory.CreateButton("Close", offlineModal.Body, "Back to the Dungeon", UiTheme.WithAlpha(UiTheme.Ember, 0.4f), UiTheme.Bone, 16);
                closeBtn.gameObject.AddComponent<LayoutElement>().preferredHeight = 44;
                closeBtn.onClick.AddListener(() => offlineModal.Hide());
                offlineModal.Show();
                BattleEvents.RaiseStateChanged();
            }

            StartCoroutine(AutosaveLoop());
        }

        IEnumerator AutosaveLoop()
        {
            var wait = new WaitForSeconds(30f);
            while (true)
            {
                yield return wait;
                SaveSystem.SaveState();
            }
        }

        void OnApplicationPause(bool paused)
        {
            if (paused) SaveSystem.SaveState();
        }

        void OnApplicationQuit()
        {
            SaveSystem.SaveState();
        }
    }
}
