using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OwnADungeon.UI
{
    // Generic centered modal, direct port of .modal-overlay/.modal (used
    // for the offline-progress summary and the reset-game confirmation).
    public class ModalPanel : MonoBehaviour
    {
        public RectTransform Body;

        public static ModalPanel Build(Transform parent, string title)
        {
            var go = new GameObject("Modal_" + title, typeof(RectTransform));
            var root = go.GetComponent<RectTransform>();
            root.SetParent(parent, false);
            UiFactory.Stretch(root);
            var modal = go.AddComponent<ModalPanel>();

            var backdrop = UiFactory.CreateImage("Backdrop", root, UiTheme.WithAlpha(Color.black, 0.65f));
            UiFactory.Stretch(backdrop.rectTransform);

            var card = UiFactory.CreateImage("Card", root, UiTheme.PanelRaised);
            UiFactory.Anchor(card.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            card.rectTransform.sizeDelta = new Vector2(420, 0);
            var outline = card.gameObject.AddComponent<Outline>();
            outline.effectColor = UiTheme.BorderBright;
            var vlayout = UiFactory.AddVerticalLayout(card.gameObject, 12, new RectOffset(20, 20, 20, 18));
            UiFactory.AddAutoHeight(card.gameObject);

            UiFactory.CreateText("Title", card.transform, title, 26, UiTheme.Gold, TextAlignmentOptions.MidlineLeft, FontStyles.Bold);

            var body = UiFactory.CreateRect("Body", card.transform);
            UiFactory.AddVerticalLayout(body.gameObject, 8, null, true, true);
            UiFactory.AddAutoHeight(body.gameObject);
            modal.Body = body;

            modal.gameObject.SetActive(false);
            return modal;
        }

        public void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);
    }
}
