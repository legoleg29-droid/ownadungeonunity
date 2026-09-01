using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OwnADungeon.UI
{
    // Reusable slide-in side panel, direct port of the .side-overlay /
    // .side-panel pattern in src/ui/overlays.ts + app/styles/components.css
    // (Armory / Upgrades / Statistics / Settings all use this). Kept
    // deliberately simple (no ScrollRect/masking) since every list this
    // wraps is short (at most ~10 rows) — reduces moving parts that can't
    // be verified without a Unity Editor available in this environment.
    public class OverlayPanel : MonoBehaviour
    {
        public RectTransform Content; // parent for the panel's own content
        RectTransform _panel;
        bool _fromLeft;
        bool _open;

        public static OverlayPanel Build(string name, Transform parent, string title, string hint, bool fromLeft, Action onClose)
        {
            var go = new GameObject(name, typeof(RectTransform));
            var root = go.GetComponent<RectTransform>();
            root.SetParent(parent, false);
            UiFactory.Stretch(root);
            var overlay = go.AddComponent<OverlayPanel>();
            overlay._fromLeft = fromLeft;

            // Backdrop
            var backdrop = UiFactory.CreateImage("Backdrop", root, UiTheme.WithAlpha(Color.black, 0.55f));
            UiFactory.Stretch(backdrop.rectTransform);
            var backdropBtn = backdrop.gameObject.AddComponent<Button>();
            backdropBtn.onClick.AddListener(() => { overlay.Close(); onClose?.Invoke(); });

            // Sliding panel: fixed 380-wide column pinned to the left or
            // right edge, full height.
            var panel = UiFactory.CreateImage("Panel", root, UiTheme.PanelRaised);
            overlay._panel = panel.rectTransform;
            float w = 380f;
            if (fromLeft)
            {
                UiFactory.Anchor(panel.rectTransform, new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, 0.5f));
            }
            else
            {
                UiFactory.Anchor(panel.rectTransform, new Vector2(1, 0), new Vector2(1, 1), new Vector2(1, 0.5f));
            }
            panel.rectTransform.sizeDelta = new Vector2(w, 0);
            panel.rectTransform.offsetMin = new Vector2(panel.rectTransform.offsetMin.x, 0);
            panel.rectTransform.offsetMax = new Vector2(panel.rectTransform.offsetMax.x, 0);

            var vlayout = UiFactory.AddVerticalLayout(panel.gameObject, 10, new RectOffset(18, 18, 18, 24));
            UiFactory.AddAutoHeight(panel.gameObject);

            // Header row
            if (!string.IsNullOrEmpty(title))
            {
                var header = UiFactory.CreateRect("Header", panel.transform);
                UiFactory.AddHorizontalLayout(header.gameObject, 12);
                var headerLE = header.gameObject.AddComponent<LayoutElement>();
                headerLE.preferredHeight = string.IsNullOrEmpty(hint) ? 40 : 64;

                var titleBlock = UiFactory.CreateRect("TitleBlock", header);
                UiFactory.AddVerticalLayout(titleBlock.gameObject, 2, null, true, true);
                var titleLE = titleBlock.gameObject.AddComponent<LayoutElement>();
                titleLE.flexibleWidth = 1;
                titleLE.preferredWidth = w - 60;
                UiFactory.CreateText("Title", titleBlock, title, 26, UiTheme.Bone, TextAlignmentOptions.MidlineLeft, FontStyles.Bold);
                if (!string.IsNullOrEmpty(hint))
                    UiFactory.CreateText("Hint", titleBlock, hint, 15, UiTheme.Muted);

                var closeBtn = UiFactory.CreateButton("Close", header, "×", UiTheme.WithAlpha(Color.black, 0.25f), UiTheme.Bone, 26);
                var closeLE = closeBtn.gameObject.AddComponent<LayoutElement>();
                closeLE.preferredWidth = 36; closeLE.preferredHeight = 36;
                closeBtn.onClick.AddListener(() => { overlay.Close(); onClose?.Invoke(); });
            }

            // Content container — callers append rows here.
            var content = UiFactory.CreateRect("Content", panel.transform);
            UiFactory.AddVerticalLayout(content.gameObject, 12, new RectOffset(0, 0, 0, 0));
            UiFactory.AddAutoHeight(content.gameObject);

            overlay.Content = content;
            overlay.SetOpen(false);
            return overlay;
        }

        public void Open() => SetOpen(true);
        public void Close() => SetOpen(false);
        public bool IsOpen => _open;

        void SetOpen(bool open)
        {
            _open = open;
            gameObject.SetActive(open);
            var backdrop = transform.Find("Backdrop");
            if (backdrop) backdrop.gameObject.SetActive(open);
        }
    }
}
