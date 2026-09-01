using TMPro;
using UnityEngine;
using OwnADungeon.State;
using OwnADungeon.Data;
using OwnADungeon.Combat;

namespace OwnADungeon.UI
{
    // Direct port of the .player-hud header block in GameApp.tsx / hud.ts.
    public class HudView : MonoBehaviour
    {
        TextMeshProUGUI _kingLevel;
        TextMeshProUGUI _progress;
        TextMeshProUGUI _gold;
        TextMeshProUGUI _souls;

        public static HudView Build(Transform parent)
        {
            var root = UiFactory.CreateRect("Hud", parent);
            var img = root.gameObject.AddComponent<UnityEngine.UI.Image>();
            img.color = UiTheme.PanelRaised;
            var le = root.gameObject.AddComponent<UnityEngine.UI.LayoutElement>();
            le.preferredHeight = 80;
            var h = UiFactory.AddHorizontalLayout(root.gameObject, 12, new RectOffset(12, 12, 8, 8));
            h.childAlignment = TextAnchor.MiddleLeft;

            var view = root.gameObject.AddComponent<HudView>();

            var avatar = UiFactory.CreateText("Avatar", root, "\U0001F451", 30, UiTheme.Gold, TextAlignmentOptions.Center);
            avatar.gameObject.AddComponent<UnityEngine.UI.LayoutElement>().preferredWidth = 44;

            var meta = UiFactory.CreateRect("Meta", root);
            UiFactory.AddVerticalLayout(meta.gameObject, 0, null, true, true);
            meta.gameObject.AddComponent<UnityEngine.UI.LayoutElement>().flexibleWidth = 1;
            UiFactory.CreateText("Name", meta, "King", 18, UiTheme.Bone, TextAlignmentOptions.MidlineLeft, FontStyles.Bold);
            view._kingLevel = UiFactory.CreateText("Level", meta, "Lv.1", 14, UiTheme.Gold);
            view._progress = UiFactory.CreateText("Progress", meta, "Stage 1 / 50", 12, UiTheme.Muted);

            var goldRow = UiFactory.CreateRect("Gold", root);
            UiFactory.AddHorizontalLayout(goldRow.gameObject, 4);
            goldRow.gameObject.AddComponent<UnityEngine.UI.LayoutElement>().preferredWidth = 90;
            UiFactory.CreateText("GoldIcon", goldRow, "\U0001FA99", 18, UiTheme.Gold, TextAlignmentOptions.Center);
            view._gold = UiFactory.CreateText("GoldValue", goldRow, "0", 18, UiTheme.Gold, TextAlignmentOptions.MidlineLeft, FontStyles.Bold);

            var soulRow = UiFactory.CreateRect("Souls", root);
            UiFactory.AddHorizontalLayout(soulRow.gameObject, 4);
            soulRow.gameObject.AddComponent<UnityEngine.UI.LayoutElement>().preferredWidth = 90;
            UiFactory.CreateText("SoulIcon", soulRow, "\U0001F47B", 18, UiTheme.Soul, TextAlignmentOptions.Center);
            view._souls = UiFactory.CreateText("SoulsValue", soulRow, "0", 18, UiTheme.Soul, TextAlignmentOptions.MidlineLeft, FontStyles.Bold);

            BattleEvents.OnStateChanged += view.Refresh;
            view.Refresh();
            return view;
        }

        public void Refresh()
        {
            var s = SaveSystem.State;
            if (s == null) return;
            var king = King.GetKingStats(s.King?.Level ?? 1);
            _kingLevel.text = $"Lv.{king.Level}";
            _gold.text = s.Gold.ToString();
            _souls.text = s.Souls.ToString();
            _progress.text = s.Mode == GameMode.Stage
                ? $"Stage {s.Stage} / {Difficulty.StageMax}"
                : $"Arcade Wave {s.ArcadeWave} (Best {s.ArcadeBest})";
        }
    }
}
