using TMPro;
using UnityEngine;
using UnityEngine.UI;
using OwnADungeon.Data;
using OwnADungeon.State;
using OwnADungeon.Economy;
using OwnADungeon.Combat;

namespace OwnADungeon.UI
{
    // Direct port of src/ui/upgradesPanel.ts.
    public class UpgradesView : MonoBehaviour
    {
        OverlayPanel _overlay;

        public static UpgradesView Build(Transform parent)
        {
            var overlay = OverlayPanel.Build("UpgradesOverlay", parent, "Upgrades", null, false, null);
            var view = overlay.gameObject.AddComponent<UpgradesView>();
            view._overlay = overlay;
            BattleEvents.OnStateChanged += view.Refresh;
            view.Refresh();
            return view;
        }

        public void Refresh()
        {
            foreach (Transform child in _overlay.Content) Destroy(child.gameObject);
            var s = SaveSystem.State;
            bool hasAny = false;

            AddSectionTitle("Raja / King");
            int kingLevel = s.King?.Level ?? 1;
            var kingStats = King.GetKingStats(kingLevel);
            var nextStats = King.GetKingStats(kingLevel + 1);
            var kingCost = King.KingUpgradeCost(kingLevel);
            bool canKing = EconomyService.Affordable(kingCost);
            string kingDesc = $"HP {kingStats.MaxHp} · ATK {kingStats.Atk} · DEF {kingStats.Def} → HP {nextStats.MaxHp} · ATK {nextStats.Atk} · DEF {nextStats.Def}";
            AddUpgradeItem("Raja (King)", $"Lv.{kingLevel}", kingDesc, "Biaya: " + EconomyService.CostLabel(kingCost), canKing, "Tingkatkan King", () =>
            {
                if (!EconomyService.TryUpgradeKing())
                {
                    BattleEvents.RaiseToast("Resource tidak cukup", ToastType.Warning);
                    return;
                }
                BattleEvents.RaiseStateChanged();
                BattleEvents.RaiseToast("King → Lv." + SaveSystem.State.King.Level, ToastType.Success);
            });
            hasAny = true;

            AddSectionTitle("Trap & Monster");
            foreach (var def in Upgrades.Defs)
            {
                if (!string.IsNullOrEmpty(def.RequiresUnlock) && !EconomyService.IsUnlocked(def.RequiresUnlock)) continue;
                hasAny = true;
                int level = s.Levels.TryGetValue(def.Id, out var l) ? l : 1;
                var cost = new Cost(EconomyService.UpgradeCost(def.BaseCost, level), 0);
                bool can = EconomyService.Affordable(cost);
                AddUpgradeItem(def.Label, $"Lv.{level}", null, "Biaya: " + EconomyService.CostLabel(cost), can, "Tingkatkan", () =>
                {
                    if (!EconomyService.Affordable(cost)) return;
                    EconomyService.Spend(cost);
                    SaveSystem.State.Levels[def.Id] = level + 1;
                    SaveSystem.SaveState();
                    BattleEvents.RaiseStateChanged();
                    BattleEvents.RaiseToast(def.Label + " → Lv." + (level + 1), ToastType.Success);
                });
            }

            foreach (var def in Upgrades.Unlocks)
            {
                if (s.Unlocked.TryGetValue(def.Id, out var already) && already) continue;
                if (def.Id == "slot4" && s.SlotCount != 3) continue;
                if (def.Id == "slot5" && s.SlotCount != 4) continue;
                if (def.UnlockAtStage > 0 && s.Stage < def.UnlockAtStage) continue;
                hasAny = true;
                bool can = EconomyService.Affordable(def.Cost);
                AddUpgradeItem(def.Label, null, null, "Biaya: " + EconomyService.CostLabel(def.Cost), can, "Buka", () =>
                {
                    if (!EconomyService.Affordable(def.Cost)) return;
                    EconomyService.Spend(def.Cost);
                    SaveSystem.State.Unlocked[def.Id] = true;
                    if (def.Id == "slot4") SaveSystem.State.SlotCount = 4;
                    if (def.Id == "slot5") SaveSystem.State.SlotCount = 5;
                    while (SaveSystem.State.Dungeon.Count < SaveSystem.State.SlotCount) SaveSystem.State.Dungeon.Add(null);
                    SaveSystem.SaveState();
                    BattleEvents.RaiseStateChanged();
                    BattleEvents.RaiseToast(def.Label + " berhasil!", ToastType.Success);
                });
            }

            if (!hasAny)
            {
                var empty = UiFactory.CreateText("Empty", _overlay.Content, "Semua item sudah terbuka & max level untuk saat ini.", 14, UiTheme.Muted, TextAlignmentOptions.Center);
                empty.gameObject.AddComponent<LayoutElement>().preferredHeight = 60;
            }
        }

        void AddSectionTitle(string text)
        {
            var t = UiFactory.CreateText("Section_" + text, _overlay.Content, text.ToUpperInvariant(), 13, UiTheme.Muted, TextAlignmentOptions.MidlineLeft, FontStyles.Bold);
            t.gameObject.AddComponent<LayoutElement>().preferredHeight = 24;
        }

        void AddUpgradeItem(string name, string levelTag, string extraDesc, string costText, bool can, string btnLabel, System.Action onClick)
        {
            var card = UiFactory.CreateImage("Upgrade_" + name, _overlay.Content, UiTheme.WithAlpha(Color.black, 0.2f));
            var v = UiFactory.AddVerticalLayout(card.gameObject, 4, new RectOffset(10, 10, 8, 8));
            UiFactory.AddAutoHeight(card.gameObject);

            var top = UiFactory.CreateRect("Top", card.transform);
            UiFactory.AddHorizontalLayout(top.gameObject, 8);
            top.gameObject.AddComponent<LayoutElement>().preferredHeight = 24;
            var nameTxt = UiFactory.CreateText("Name", top, name, 16, UiTheme.Bone, TextAlignmentOptions.MidlineLeft, FontStyles.Bold);
            nameTxt.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1;
            if (!string.IsNullOrEmpty(levelTag))
                UiFactory.CreateText("Level", top, levelTag, 14, UiTheme.Gold);

            if (!string.IsNullOrEmpty(extraDesc))
                UiFactory.CreateText("Desc", card.transform, extraDesc, 12, UiTheme.Muted);
            UiFactory.CreateText("Cost", card.transform, costText, 12, UiTheme.Muted);

            var btn = UiFactory.CreateButton("Btn", card.transform, btnLabel, can ? UiTheme.WithAlpha(UiTheme.Ember, 0.35f) : UiTheme.WithAlpha(UiTheme.Border, 0.5f), UiTheme.Bone, 14);
            btn.gameObject.AddComponent<LayoutElement>().preferredHeight = 36;
            btn.interactable = can;
            btn.onClick.AddListener(() => onClick());
        }
    }
}
