using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using OwnADungeon.Data;
using OwnADungeon.State;
using OwnADungeon.Economy;
using OwnADungeon.Combat;

namespace OwnADungeon.UI
{
    // Direct port of src/ui/palette.ts — the Armory overlay: pick a trap,
    // monster, or the treasure room, then tap an empty dungeon slot.
    public class PaletteView : MonoBehaviour
    {
        OverlayPanel _overlay;

        public static PaletteView Build(Transform parent, System.Action onCloseAll)
        {
            var overlay = OverlayPanel.Build("PaletteOverlay", parent, "Dungeon Armory",
                "Select an item, then tap an empty dungeon room to place it.", true, null);
            var view = overlay.gameObject.AddComponent<PaletteView>();
            view._overlay = overlay;

            BattleEvents.OnStateChanged += view.Refresh;
            view.Refresh();
            return view;
        }

        public void Refresh()
        {
            foreach (Transform child in _overlay.Content) Destroy(child.gameObject);

            AddGroup("Trap", Catalog.Traps.Values.Cast<CatalogItem>().Where(i => EconomyService.IsUnlocked(i.Id)).ToList(), "No traps unlocked yet.\nUnlock them in Upgrades.");
            AddGroup("Monster", Catalog.Monsters.Values.Cast<CatalogItem>().Where(i => EconomyService.IsUnlocked(i.Id)).ToList(), "No monsters unlocked yet.\nUnlock them in Upgrades.");
            AddGroup("Special Rooms", new System.Collections.Generic.List<CatalogItem> { Catalog.Treasure }, null);
        }

        void AddGroup(string title, System.Collections.Generic.List<CatalogItem> items, string emptyText)
        {
            var groupTitle = UiFactory.CreateText("GroupTitle_" + title, _overlay.Content, title.ToUpperInvariant(), 14, UiTheme.Muted, TextAlignmentOptions.MidlineLeft, FontStyles.Bold);
            groupTitle.gameObject.AddComponent<LayoutElement>().preferredHeight = 22;

            if (items.Count == 0 && emptyText != null)
            {
                var empty = UiFactory.CreateText("Empty_" + title, _overlay.Content, emptyText, 14, UiTheme.Muted, TextAlignmentOptions.Center);
                empty.gameObject.AddComponent<LayoutElement>().preferredHeight = 50;
                return;
            }

            foreach (var item in items)
            {
                AddItem(item);
            }
        }

        void AddItem(CatalogItem item)
        {
            bool unlocked = item.Kind == ItemKind.Treasure || EconomyService.IsUnlocked(item.Id);
            bool selected = RuntimeState.SelectedPaletteItem != null && RuntimeState.SelectedPaletteItem.Id == item.Id;

            var bg = selected ? UiTheme.WithAlpha(UiTheme.Gold, 0.18f) : UiTheme.WithAlpha(Color.black, 0.22f);
            var row = UiFactory.CreateImage("Item_" + item.Id, _overlay.Content, bg);
            row.gameObject.AddComponent<LayoutElement>().preferredHeight = 66;
            var h = UiFactory.AddHorizontalLayout(row.gameObject, 10, new RectOffset(10, 10, 8, 8));
            h.childAlignment = TextAnchor.MiddleLeft;

            var icon = UiFactory.CreateText("Icon", row.transform, item.Icon, 26, UiTheme.Bone, TextAlignmentOptions.Center);
            icon.gameObject.AddComponent<LayoutElement>().preferredWidth = 40;

            var info = UiFactory.CreateRect("Info", row.transform);
            UiFactory.AddVerticalLayout(info.gameObject, 2, null, true, true);
            info.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1;
            UiFactory.CreateText("Name", info, item.Name, 16, UiTheme.Bone, TextAlignmentOptions.MidlineLeft, FontStyles.Bold);
            string desc = unlocked ? item.Desc : "Locked — unlock in Upgrades";
            UiFactory.CreateText("Desc", info, desc, 12, UiTheme.Muted);

            if (unlocked && item.Kind != ItemKind.Treasure)
            {
                int lvl = SaveSystem.State.Levels.TryGetValue(item.Id, out var l) ? l : 1;
                var lvlTxt = UiFactory.CreateText("Lvl", row.transform, "Lv." + lvl, 14, UiTheme.Gold);
                lvlTxt.gameObject.AddComponent<LayoutElement>().preferredWidth = 44;
            }

            if (unlocked)
            {
                var btn = row.gameObject.AddComponent<Button>();
                btn.onClick.AddListener(() =>
                {
                    if (RuntimeState.SelectedPaletteItem != null && RuntimeState.SelectedPaletteItem.Id == item.Id)
                        RuntimeState.SelectedPaletteItem = null;
                    else
                        RuntimeState.SelectedPaletteItem = new SelectedPaletteItem { Id = item.Id, Kind = item.Kind };

                    Refresh();
                    if (RuntimeState.SelectedPaletteItem != null)
                    {
                        _overlay.Close();
                        BattleEvents.RaiseToast(item.Name + " selected — tap an empty slot", ToastType.Info);
                    }
                });
            }
        }
    }
}
