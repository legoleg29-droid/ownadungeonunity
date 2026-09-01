using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using OwnADungeon.Data;
using OwnADungeon.State;
using OwnADungeon.Combat;

namespace OwnADungeon.UI
{
    // Direct port of src/ui/dungeonSlots.ts — entrance, N item slots
    // (locked slots shown as "Dig"), and the throne room.
    public class DungeonSlotsView : MonoBehaviour
    {
        RectTransform _wrap;
        readonly List<Image> _slotImages = new List<Image>();
        Image _throneImage;

        public static DungeonSlotsView Build(Transform parent)
        {
            var root = UiFactory.CreateRect("DungeonSlots", parent);
            var h = UiFactory.AddHorizontalLayout(root.gameObject, 6, new RectOffset(4, 4, 4, 4));
            h.childAlignment = TextAnchor.MiddleLeft;
            var le = root.gameObject.AddComponent<LayoutElement>();
            le.preferredHeight = 110;
            // A horizontally-scrolling row (5 slots + entrance/throne can
            // overflow a narrow phone width) needs a proper Viewport+
            // Content split to work with ScrollRect; that's more moving
            // parts than is safe to hand-author without an Editor to
            // verify it in. Left as a plain overflowing row for now — see
            // the migration report's "not fully migrated" list.
            var view = root.gameObject.AddComponent<DungeonSlotsView>();
            view._wrap = root;

            BattleEvents.OnStateChanged += view.Refresh;
            BattleEvents.OnFlashSlot += view.OnFlash;
            view.Refresh();
            return view;
        }

        public void Refresh()
        {
            foreach (Transform child in _wrap) Destroy(child.gameObject);
            _slotImages.Clear();

            var s = SaveSystem.State;

            AddSlot("Entrance", "🚪", UiTheme.Gold, null);

            for (int i = 0; i < s.MaxSlotCount; i++)
            {
                if (i > 0) AddConnector();
                bool locked = i >= s.SlotCount;
                var slotData = i < s.Dungeon.Count ? s.Dungeon[i] : null;

                if (locked)
                {
                    AddSlot("Locked", "⛏", UiTheme.Muted, null);
                }
                else if (slotData != null)
                {
                    var cat = Catalog.CatalogFor(slotData.CatalogId, slotData.Kind);
                    int idx = i;
                    var img = AddSlot(cat != null ? cat.Name : "Item", cat != null ? cat.Icon : "?", UiTheme.Bone, () =>
                    {
                        if (RuntimeState.RaidInProgress) return;
                        s.Dungeon[idx] = null;
                        SaveSystem.SaveState();
                        BattleEvents.RaiseStateChanged();
                        BattleEvents.RaiseToast((cat != null ? cat.Name : "Item") + " removed", ToastType.Info);
                    });
                    _slotImages.Add(img);
                }
                else
                {
                    int idx = i;
                    var img = AddSlot("Empty", "·", UiTheme.MutedDim, () =>
                    {
                        if (RuntimeState.RaidInProgress || RuntimeState.SelectedPaletteItem == null) return;
                        var sel = RuntimeState.SelectedPaletteItem;
                        var cat = Catalog.CatalogFor(sel.Id, sel.Kind);
                        s.Dungeon[idx] = new DungeonSlotData { CatalogId = sel.Id, Kind = sel.Kind };
                        RuntimeState.SelectedPaletteItem = null;
                        SaveSystem.SaveState();
                        BattleEvents.RaiseStateChanged();
                        BattleEvents.RaiseToast((cat != null ? cat.Name : "Item") + " placed", ToastType.Success);
                    });
                    _slotImages.Add(img);
                }
            }

            AddConnector();
            int kingLv = s.King?.Level ?? 1;
            _throneImage = AddSlot("Throne", "👑", UiTheme.Gold, null, $"Chest · King Lv.{kingLv}");
        }

        void AddConnector()
        {
            var c = UiFactory.CreateImage("Connector", _wrap, UiTheme.Border);
            c.gameObject.AddComponent<LayoutElement>().preferredWidth = 12;
            c.rectTransform.sizeDelta = new Vector2(12, 2);
        }

        Image AddSlot(string label, string icon, Color iconColor, System.Action onClick, string sub = null)
        {
            var slot = UiFactory.CreateImage("Slot", _wrap, UiTheme.PanelRaised);
            var le = slot.gameObject.AddComponent<LayoutElement>();
            le.preferredWidth = 88; le.preferredHeight = 100;
            var v = UiFactory.AddVerticalLayout(slot.gameObject, 2, new RectOffset(4, 4, 6, 6));
            v.childAlignment = TextAnchor.MiddleCenter;

            UiFactory.CreateText("Icon", slot.transform, icon, 26, iconColor, TextAlignmentOptions.Center);
            var labelTxt = UiFactory.CreateText("Label", slot.transform, label, 12, UiTheme.Bone, TextAlignmentOptions.Center);
            labelTxt.textWrappingMode = TextWrappingModes.NoWrap;
            labelTxt.overflowMode = TextOverflowModes.Ellipsis;
            if (!string.IsNullOrEmpty(sub))
                UiFactory.CreateText("Sub", slot.transform, sub, 9, UiTheme.Muted, TextAlignmentOptions.Center);

            if (onClick != null)
            {
                var btn = slot.gameObject.AddComponent<Button>();
                btn.onClick.AddListener(() => onClick());
            }
            return slot;
        }

        void OnFlash(int index, SlotFlash flash)
        {
            // index: -1 => throne highlight lifecycle handled via ClearAll,
            // -2 => throne. 0..N => item slot (offset by the Entrance slot
            // which occupies _slotImages[-1]... _slotImages holds only the
            // item + throne slots in order, so index maps 1:1 except the
            // throne which is appended last).
            Image target = null;
            if (flash == SlotFlash.ClearAll)
            {
                foreach (var img in _slotImages) img.color = UiTheme.PanelRaised;
                if (_throneImage) _throneImage.color = UiTheme.PanelRaised;
                return;
            }
            if (index == -2) target = _throneImage;
            else if (index >= 0 && index < _slotImages.Count) target = _slotImages[index];
            if (target == null) return;

            switch (flash)
            {
                case SlotFlash.Triggered: target.color = UiTheme.WithAlpha(UiTheme.Ember, 0.5f); break;
                case SlotFlash.Cleared: target.color = UiTheme.WithAlpha(UiTheme.Success, 0.5f); break;
                case SlotFlash.Kill: target.color = UiTheme.WithAlpha(UiTheme.Danger, 0.65f); break;
                case SlotFlash.RaidActive: target.color = UiTheme.WithAlpha(UiTheme.Gold, 0.35f); break;
                case SlotFlash.RaidCleared: target.color = UiTheme.PanelRaised; break;
            }
        }
    }
}
