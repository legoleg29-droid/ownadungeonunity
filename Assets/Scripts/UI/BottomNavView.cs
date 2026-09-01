using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OwnADungeon.UI
{
    // Direct port of the .bottom-nav bar in GameApp.tsx: Armory, Upgrade,
    // Home (no-op, just visual "current tab"), Stats, Settings.
    public class BottomNavView : MonoBehaviour
    {
        public static void Build(Transform parent, OverlayPanel palette, OverlayPanel upgrades, OverlayPanel stats, OverlayPanel settings)
        {
            // "parent" here is the raw "app" frame (no layout group), so
            // this pins itself to the bottom edge directly — matching the
            // web version's position:fixed/absolute .bottom-nav — rather
            // than relying on a LayoutElement inside a flow that doesn't
            // exist at this level.
            var root = UiFactory.CreateImage("BottomNav", parent, UiTheme.PanelRaised);
            UiFactory.Anchor(root.rectTransform, new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 0));
            root.rectTransform.sizeDelta = new Vector2(0, 76);
            root.rectTransform.anchoredPosition = Vector2.zero;
            var h = UiFactory.AddHorizontalLayout(root.gameObject, 4, new RectOffset(4, 4, 6, 6));
            h.childAlignment = TextAnchor.MiddleCenter;
            h.childForceExpandWidth = true;

            AddItem(root.transform, "🗡", "Armory", () => palette.Open());
            AddItem(root.transform, "⬆", "Upgrade", () => upgrades.Open());
            AddItem(root.transform, "🏭", "Battle", null);
            AddItem(root.transform, "📊", "Stats", () => stats.Open());
            AddItem(root.transform, "⚙", "Settings", () => settings.Open());
        }

        static void AddItem(Transform parent, string icon, string label, System.Action onClick)
        {
            var item = UiFactory.CreateRect("Nav_" + label, parent);
            var v = UiFactory.AddVerticalLayout(item.gameObject, 2, null, true, true);
            v.childAlignment = TextAnchor.MiddleCenter;
            item.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1;

            UiFactory.CreateText("Icon", item, icon, 22, UiTheme.Muted, TextAlignmentOptions.Center);
            UiFactory.CreateText("Label", item, label, 10, UiTheme.Muted, TextAlignmentOptions.Center);

            if (onClick != null)
            {
                var btn = item.gameObject.AddComponent<Button>();
                btn.onClick.AddListener(() => onClick());
            }
        }
    }
}
