using TMPro;
using UnityEngine;
using UnityEngine.UI;
using OwnADungeon.Data;
using OwnADungeon.State;
using OwnADungeon.Core;
using OwnADungeon.Combat;

namespace OwnADungeon.UI
{
    // Direct port of the Settings overlay in app/GameApp.tsx + the reset
    // confirmation modal wiring in game-client.ts.
    public class SettingsView : MonoBehaviour
    {
        public static SettingsView Build(Transform parent, ModalPanel resetModal)
        {
            var overlay = OverlayPanel.Build("SettingsOverlay", parent, "Settings", "Preferences and data", false, null);
            var view = overlay.gameObject.AddComponent<SettingsView>();

            var langSection = SectionBox(overlay.Content, "Language");
            UiFactory.CreateText("LangValue", langSection, "Display language: English", 14, UiTheme.Bone);
            UiFactory.CreateText("LangNote", langSection, "English only for now. More languages later.", 12, UiTheme.Muted);

            var dangerSection = SectionBox(overlay.Content, "Danger zone", UiTheme.WithAlpha(UiTheme.Danger, 0.08f));
            UiFactory.CreateText("DangerNote", dangerSection, "Deletes gold, souls, upgrades, unlocks, layout, and stats. Cannot be undone.", 12, UiTheme.Muted);
            var resetBtn = UiFactory.CreateButton("ResetBtn", dangerSection, "Reset game", UiTheme.WithAlpha(UiTheme.Danger, 0.3f), UiTheme.Bone, 16);
            resetBtn.gameObject.AddComponent<LayoutElement>().preferredHeight = 44;
            resetBtn.onClick.AddListener(() =>
            {
                if (RuntimeState.RaidInProgress)
                {
                    BattleEvents.RaiseToast("Tunggu raid selesai dulu", ToastType.Warning);
                    return;
                }
                resetModal.Show();
            });

            return view;
        }

        static RectTransform SectionBox(RectTransform parent, string title, Color? bg = null)
        {
            var box = UiFactory.CreateImage("Section_" + title, parent, bg ?? UiTheme.WithAlpha(Color.white, 0.03f));
            var v = UiFactory.AddVerticalLayout(box.gameObject, 6, new RectOffset(12, 12, 10, 12));
            UiFactory.AddAutoHeight(box.gameObject);
            UiFactory.CreateText("Title", box.transform, title.ToUpperInvariant(), 13, UiTheme.Muted, TextAlignmentOptions.MidlineLeft, FontStyles.Bold);
            return box.rectTransform;
        }
    }
}
