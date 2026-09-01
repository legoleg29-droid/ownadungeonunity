using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OwnADungeon.UI
{
    // Small helpers for building the entire uGUI hierarchy from code at
    // runtime (see GameController). Building UI this way — rather than
    // hand-authoring prefabs/scene YAML with no Unity Editor available to
    // verify them — avoids shipping a scene that fails to open or has
    // dangling GUID references. It mirrors how the web version built its
    // DOM from GameApp.tsx + game-client.ts rather than a static index.html.
    public static class UiFactory
    {
        public static RectTransform CreateRect(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            var rt = go.GetComponent<RectTransform>();
            rt.SetParent(parent, false);
            return rt;
        }

        public static RectTransform Stretch(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            return rt;
        }

        public static Image CreateImage(string name, Transform parent, Color color)
        {
            var rt = CreateRect(name, parent);
            var img = rt.gameObject.AddComponent<Image>();
            img.color = color;
            return img;
        }

        public static TextMeshProUGUI CreateText(string name, Transform parent, string text, int size, Color color,
            TextAlignmentOptions align = TextAlignmentOptions.MidlineLeft, FontStyles style = FontStyles.Normal)
        {
            var rt = CreateRect(name, parent);
            var tmp = rt.gameObject.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = size;
            tmp.color = color;
            tmp.alignment = align;
            tmp.fontStyle = style;
            tmp.textWrappingMode = TextWrappingModes.Normal;
            return tmp;
        }

        // Pill-shaped button matching the web's .btn styling (parchment/
        // ember chrome is applied by the caller via `bg`/`border`).
        public static Button CreateButton(string name, Transform parent, string label, Color bg, Color textColor, int fontSize = 24)
        {
            var img = CreateImage(name, parent, bg);
            var btn = img.gameObject.AddComponent<Button>();
            var colors = btn.colors;
            colors.normalColor = bg;
            colors.highlightedColor = new Color(bg.r * 1.15f, bg.g * 1.15f, bg.b * 1.15f, bg.a);
            colors.pressedColor = new Color(bg.r * 0.8f, bg.g * 0.8f, bg.b * 0.8f, bg.a);
            colors.disabledColor = UiTheme.WithAlpha(bg, 0.45f);
            btn.colors = colors;

            var txt = CreateText(name + "_Label", img.transform, label, fontSize, textColor, TextAlignmentOptions.Center);
            Stretch(txt.rectTransform);
            return btn;
        }

        public static VerticalLayoutGroup AddVerticalLayout(GameObject go, int spacing = 8, RectOffset padding = null,
            bool controlW = true, bool controlH = false)
        {
            var v = go.AddComponent<VerticalLayoutGroup>();
            v.spacing = spacing;
            v.padding = padding ?? new RectOffset(0, 0, 0, 0);
            v.childControlWidth = controlW;
            v.childControlHeight = controlH;
            v.childForceExpandWidth = controlW;
            v.childForceExpandHeight = false;
            return v;
        }

        public static HorizontalLayoutGroup AddHorizontalLayout(GameObject go, int spacing = 8, RectOffset padding = null)
        {
            var h = go.AddComponent<HorizontalLayoutGroup>();
            h.spacing = spacing;
            h.padding = padding ?? new RectOffset(0, 0, 0, 0);
            h.childControlWidth = false;
            h.childControlHeight = true;
            h.childForceExpandWidth = false;
            h.childForceExpandHeight = true;
            return h;
        }

        public static ContentSizeFitter AddAutoHeight(GameObject go)
        {
            var f = go.AddComponent<ContentSizeFitter>();
            f.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            return f;
        }

        public static void SetSize(RectTransform rt, float w, float h)
        {
            rt.sizeDelta = new Vector2(w, h);
        }

        public static void Anchor(RectTransform rt, Vector2 min, Vector2 max, Vector2 pivot)
        {
            rt.anchorMin = min;
            rt.anchorMax = max;
            rt.pivot = pivot;
        }
    }
}
