using TMPro;
using UnityEngine;
using UnityEngine.UI;
using OwnADungeon.Data;
using OwnADungeon.State;
using OwnADungeon.Combat;

namespace OwnADungeon.UI
{
    // Direct port of src/ui/statsPanel.ts.
    public class StatsView : MonoBehaviour
    {
        OverlayPanel _overlay;

        public static StatsView Build(Transform parent)
        {
            var overlay = OverlayPanel.Build("StatsOverlay", parent, "Statistics", null, false, null);
            var view = overlay.gameObject.AddComponent<StatsView>();
            view._overlay = overlay;
            BattleEvents.OnStateChanged += view.Refresh;
            view.Refresh();
            return view;
        }

        public void Refresh()
        {
            foreach (Transform child in _overlay.Content) Destroy(child.gameObject);
            var s = SaveSystem.State;
            var king = King.GetKingStats(s.King?.Level ?? 1);

            Section("Resources");
            Row("Gold", s.Gold.ToString());
            Row("Souls", s.Souls.ToString());
            Section("Mode & Progress");
            Row("Mode", s.Mode == GameMode.Arcade ? "Arcade" : "Stage");
            Row("Stage", $"{s.Stage} / {Difficulty.StageMax}");
            Row("Stage Tertinggi", s.MaxStageCleared.ToString());
            Row("Arcade Wave", s.ArcadeWave.ToString());
            Row("Arcade Best", s.ArcadeBest.ToString());
            Section("King");
            Row("King Level", king.Level.ToString());
            Row("King HP", king.MaxHp.ToString());
            Row("King ATK", king.Atk.ToString());
            Row("King DEF", king.Def.ToString());
            Section("Raid Stats");
            Row("Total Raid", s.Stats.RaidsTotal.ToString());
            Row("Dungeon Menang", s.Stats.DungeonWins.ToString());
            Row("Hero Kabur", s.Stats.HeroEscapes.ToString());
            Row("Hero Menang", s.Stats.HeroVictories.ToString());
        }

        void Section(string text)
        {
            var t = UiFactory.CreateText("Section_" + text, _overlay.Content, text.ToUpperInvariant(), 13, UiTheme.Muted, TextAlignmentOptions.MidlineLeft, FontStyles.Bold);
            t.gameObject.AddComponent<LayoutElement>().preferredHeight = 26;
        }

        void Row(string label, string value)
        {
            var row = UiFactory.CreateRect("Row_" + label, _overlay.Content);
            UiFactory.AddHorizontalLayout(row.gameObject, 8);
            row.gameObject.AddComponent<LayoutElement>().preferredHeight = 28;
            var l = UiFactory.CreateText("Label", row, label, 14, UiTheme.Bone);
            l.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1;
            UiFactory.CreateText("Value", row, value, 14, UiTheme.Bone, TextAlignmentOptions.MidlineRight);
        }
    }
}
