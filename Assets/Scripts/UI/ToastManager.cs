using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using OwnADungeon.Data;
using OwnADungeon.Combat;

namespace OwnADungeon.UI
{
    // Direct port of src/ui/toast.ts — a stack of auto-dismissing toast
    // messages anchored above the bottom nav.
    public class ToastManager : MonoBehaviour
    {
        RectTransform _container;

        public void Build(Transform parent)
        {
            _container = UiFactory.CreateRect("ToastContainer", parent);
            UiFactory.Anchor(_container, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f));
            _container.anchoredPosition = new Vector2(0, 150f);
            UiFactory.SetSize(_container, 440, 300);
            var v = UiFactory.AddVerticalLayout(_container.gameObject, 8, new RectOffset(0, 0, 0, 0), true, true);
            v.childAlignment = TextAnchor.LowerCenter;
            UiFactory.AddAutoHeight(_container.gameObject);

            BattleEvents.OnToast += (text, type) => Show(text, type);
        }

        public void Show(string text, ToastType type)
        {
            var color = type == ToastType.Warning ? UiTheme.Ember : type == ToastType.Success ? UiTheme.Gold : UiTheme.Border;
            var toast = UiFactory.CreateImage("Toast", _container, UiTheme.WithAlpha(new Color(0.08f, 0.06f, 0.05f), 0.95f));
            var le = toast.gameObject.AddComponent<LayoutElement>();
            le.preferredHeight = 44;
            le.preferredWidth = 400;
            var outline = toast.gameObject.AddComponent<Outline>();
            outline.effectColor = color;
            outline.effectDistance = new Vector2(1, 1);
            var txt = UiFactory.CreateText("Text", toast.transform, text, 20, UiTheme.Bone, TextAlignmentOptions.MidlineLeft);
            UiFactory.Stretch(txt.rectTransform);
            txt.margin = new Vector4(14, 4, 14, 4);
            StartCoroutine(AutoDismiss(toast.gameObject));
        }

        IEnumerator AutoDismiss(GameObject go)
        {
            yield return new WaitForSeconds(2.6f);
            if (go) Destroy(go);
        }
    }
}
