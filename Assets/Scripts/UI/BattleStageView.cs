using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using OwnADungeon.Data;
using OwnADungeon.State;
using OwnADungeon.Combat;
using OwnADungeon.Animation;

namespace OwnADungeon.UI
{
    // Consolidated port of src/ui/roomPreview.ts + src/ui/battleReaction.ts
    // + src/animation/roomStage.ts + heroToken.ts/monsterToken.ts. These
    // are four small, tightly-coupled DOM-manipulation modules in the web
    // version (all driving the same #room-preview / #room-stage DOM
    // subtree); merging them into one view class avoids five near-empty
    // Unity files calling back and forth for what is one presentation
    // surface, while keeping the same information flow as the original.
    public class BattleStageView : MonoBehaviour
    {
        // Pre-raid "Enemy Detected" / mid-raid compact battle card.
        TextMeshProUGUI _cardTitle, _cardIcon, _cardName, _cardClass, _cardStats, _cardHint, _cardReaction, _cardHpText;
        Image _cardHpFill;
        RectTransform _cardHpBar;
        GameObject _battleCardBlock;
        GameObject _introBlock;
        TextMeshProUGUI _introBlurb, _introTraits, _introMatchups;

        // Room stage (lane view).
        TextMeshProUGUI _roomDepth;
        TextMeshProUGUI _doorState;
        RectTransform _floor;
        TextMeshProUGUI _heroToken;
        TextMeshProUGUI _monsterToken;
        bool _heroVisible;
        bool _monsterVisible;
        Hero _currentHero;

        public static BattleStageView Build(Transform previewParent, Transform stageParent)
        {
            var view = previewParent.gameObject.AddComponent<BattleStageView>();
            view.BuildPreviewCard(previewParent);
            view.BuildRoomStage(stageParent);

            BattleEvents.OnStateChanged += view.RenderRoomPreviewIdle;
            BattleEvents.OnHeroIntro += view.ShowHeroIntro;
            BattleEvents.OnShowBattleCard += view.ShowBattleCard;
            BattleEvents.OnUpdateBattleCard += view.UpdateBattleCard;
            BattleEvents.OnBattleReaction += view.SetReaction;
            BattleEvents.OnHeroVisualSync += view.SyncVisual;

            BattleEvents.OnPresentRoom += view.PresentRoom;
            BattleEvents.OnDoorOpen += view.SetDoorOpen;
            BattleEvents.OnShowHeroToken += view.ShowHeroToken;
            BattleEvents.OnHideHeroToken += view.HideHeroToken;
            BattleEvents.OnShowMonsterToken += view.ShowMonsterToken;
            BattleEvents.OnHideMonsterToken += view.HideMonsterToken;
            BattleEvents.OnHeroWalkToExit += view.HeroWalkToExit;

            view.RenderRoomPreviewIdle();
            return view;
        }

        // ---------------------------------------------------------------
        // Pre-raid intro card / mid-raid compact battle card
        // ---------------------------------------------------------------

        void BuildPreviewCard(Transform parent)
        {
            var v = UiFactory.AddVerticalLayout(parent.gameObject, 6, new RectOffset(10, 10, 8, 10));
            UiFactory.AddAutoHeight(parent.gameObject);

            _cardTitle = UiFactory.CreateText("PreviewTitle", parent, "Enemy Detected", 14, UiTheme.Muted, TextAlignmentOptions.MidlineLeft, FontStyles.Bold);

            // Intro block
            _introBlock = new GameObject("IntroBlock", typeof(RectTransform));
            _introBlock.transform.SetParent(parent, false);
            UiFactory.AddVerticalLayout(_introBlock, 4, null, true, true);
            UiFactory.AddAutoHeight(_introBlock);

            var headRow = UiFactory.CreateRect("HeadRow", _introBlock.transform);
            UiFactory.AddHorizontalLayout(headRow.gameObject, 10);
            headRow.gameObject.AddComponent<LayoutElement>().preferredHeight = 40;
            _cardIcon = UiFactory.CreateText("Icon", headRow, "⚔", 28, UiTheme.Bone, TextAlignmentOptions.Center);
            _cardIcon.gameObject.AddComponent<LayoutElement>().preferredWidth = 40;
            var nameBlock = UiFactory.CreateRect("NameBlock", headRow);
            UiFactory.AddVerticalLayout(nameBlock.gameObject, 0, null, true, true);
            nameBlock.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1;
            _cardName = UiFactory.CreateText("Name", nameBlock, "-", 16, UiTheme.Bone, TextAlignmentOptions.MidlineLeft, FontStyles.Bold);
            _cardClass = UiFactory.CreateText("Class", nameBlock, "-", 12, UiTheme.Gold);

            _cardStats = UiFactory.CreateText("Stats", _introBlock.transform, "", 13, UiTheme.Muted);
            _introBlurb = UiFactory.CreateText("Blurb", _introBlock.transform, "", 12, UiTheme.Muted);
            _introTraits = UiFactory.CreateText("Traits", _introBlock.transform, "", 12, UiTheme.Gold);
            _introMatchups = UiFactory.CreateText("Matchups", _introBlock.transform, "", 12, UiTheme.Bone);
            _cardHint = UiFactory.CreateText("Hint", _introBlock.transform, "", 12, UiTheme.Muted);

            // Battle card block
            _battleCardBlock = new GameObject("BattleCardBlock", typeof(RectTransform));
            _battleCardBlock.transform.SetParent(parent, false);
            UiFactory.AddVerticalLayout(_battleCardBlock, 4, null, true, true);
            UiFactory.AddAutoHeight(_battleCardBlock);

            var barBg = UiFactory.CreateImage("HpBarBg", _battleCardBlock.transform, UiTheme.WithAlpha(Color.black, 0.4f));
            barBg.gameObject.AddComponent<LayoutElement>().preferredHeight = 14;
            _cardHpFill = UiFactory.CreateImage("HpFill", barBg.transform, UiTheme.Success);
            _cardHpBar = _cardHpFill.rectTransform;
            UiFactory.Anchor(_cardHpBar, new Vector2(0, 0), new Vector2(1, 1), new Vector2(0, 0.5f));
            _cardHpBar.offsetMin = Vector2.zero; _cardHpBar.offsetMax = Vector2.zero;

            var statsRow = UiFactory.CreateRect("StatsRow", _battleCardBlock.transform);
            UiFactory.AddHorizontalLayout(statsRow.gameObject, 8);
            statsRow.gameObject.AddComponent<LayoutElement>().preferredHeight = 20;
            var lvlTxt = UiFactory.CreateText("Level", statsRow, "Lv.1", 12, UiTheme.Muted);
            lvlTxt.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1;
            _cardHpText = UiFactory.CreateText("HpText", statsRow, "HP 0/0", 12, UiTheme.Muted, TextAlignmentOptions.MidlineRight);

            _cardReaction = UiFactory.CreateText("Reaction", _battleCardBlock.transform, "", 16, UiTheme.EmberBright, TextAlignmentOptions.Center, FontStyles.Bold);
            _cardReaction.gameObject.AddComponent<LayoutElement>().preferredHeight = 22;

            _battleCardBlock.SetActive(false);
        }

        public void RenderRoomPreviewIdle()
        {
            if (RuntimeState.RaidInProgress) return;
            var hero = HeroFactory.EnsurePendingHero();
            RenderIntro(hero, "Build traps and monsters against this enemy weaknesses, then start the Raid.");
        }

        void ShowHeroIntro(Hero hero)
        {
            RenderIntro(hero, "Raid started — watch their reactions as the fight begins.");
        }

        void RenderIntro(Hero hero, string hint)
        {
            _cardTitle.text = "Enemy Detected";
            _introBlock.SetActive(true);
            _battleCardBlock.SetActive(false);

            _cardIcon.text = hero.Icon;
            _cardName.text = hero.Name;
            _cardClass.text = hero.ClassName;
            _cardStats.text = $"Lv.{hero.Level} · HP {hero.MaxHp} · ATK {hero.Atk} · DEF {hero.Def}";
            _introBlurb.text = (hero.Strengths ?? "") + (string.IsNullOrEmpty(hero.Weaknesses) ? "" : "  |  " + hero.Weaknesses);

            var traits = new System.Collections.Generic.List<string>();
            if (hero.FearImmune) traits.Add("Fear Immune");
            if (hero.CanRage) traits.Add("RAGE");
            if (hero.TrapEvasion >= 0.3f) traits.Add("Trap Evasion");
            if (hero.MagicAtk) traits.Add("Magic ATK");
            if (hero.Holy) traits.Add("Holy");
            _introTraits.text = string.Join("   ", traits);

            _introMatchups.text = BuildMatchupHints(hero);
            _cardHint.text = hint;
        }

        string BuildMatchupHints(Hero hero)
        {
            var s = SaveSystem.State;
            var bits = new System.Collections.Generic.List<string>();
            int count = Mathf.Min(s.SlotCount, s.Dungeon.Count);
            for (int i = 0; i < count; i++)
            {
                var slot = s.Dungeon[i];
                if (slot == null) continue;
                var cat = Catalog.CatalogFor(slot.CatalogId, slot.Kind);
                if (cat == null) continue;
                float mult = 1f;
                if (slot.Kind == ItemKind.Monster) mult = Matchups.HeroMonsterMult(hero.ClassId, cat.Id);
                else if (slot.Kind == ItemKind.Trap) mult = Matchups.HeroTrapMult(hero.ClassId, cat.Id);
                else continue;
                var label = Matchups.GetMatchupLabel(mult);
                string tip = label == MatchupLabel.Strong ? (slot.Kind == ItemKind.Trap ? "dangerous" : "hero favored")
                    : label == MatchupLabel.Weak ? (slot.Kind == ItemKind.Trap ? "weak" : "hero struggles")
                    : "neutral";
                bits.Add($"{cat.Icon} R{i + 1} {tip}");
            }
            return bits.Count > 0 ? string.Join("   ", bits) : "Place traps/monsters in the Armory to see matchups here.";
        }

        void ShowBattleCard(Hero hero)
        {
            _currentHero = hero;
            _cardTitle.text = "Battle";
            _introBlock.SetActive(false);
            _battleCardBlock.SetActive(true);
            UpdateBattleCard(hero);
        }

        void UpdateBattleCard(Hero hero)
        {
            _currentHero = hero;
            if (!_battleCardBlock.activeSelf) return;
            float pct = hero.MaxHp > 0 ? Mathf.Clamp(hero.Hp / (float)hero.MaxHp, 0f, 1f) : 0f;
            _cardHpBar.anchorMax = new Vector2(pct, 1f);
            _cardHpFill.color = pct <= 0.35f ? UiTheme.Danger : pct <= 0.65f ? UiTheme.Gold : UiTheme.Success;
            _cardHpText.text = $"HP {Mathf.Max(0, hero.Hp)}/{hero.MaxHp}";
        }

        void SetReaction(string text, ReactionKind kind)
        {
            _cardReaction.text = text ?? "";
            _cardReaction.color = kind switch
            {
                ReactionKind.Rage => UiTheme.EmberBright,
                ReactionKind.Panic => UiTheme.Danger,
                ReactionKind.Flee => UiTheme.Muted,
                ReactionKind.Fear => UiTheme.Soul,
                ReactionKind.Pain => UiTheme.Danger,
                ReactionKind.Dead => UiTheme.Danger,
                _ => UiTheme.Bone
            };
        }

        void SyncVisual(Hero hero)
        {
            _currentHero = hero;
            string icon = hero.VisualState switch
            {
                HeroVisualState.Dead => "💀",
                HeroVisualState.Flee => "💨",
                HeroVisualState.Rage => "🔥",
                HeroVisualState.Panic => "😰",
                _ => hero.Icon
            };
            if (_battleCardBlock.activeSelf) _cardIcon.text = icon;
            if (_heroToken) _heroToken.text = icon;
        }

        // ---------------------------------------------------------------
        // Room stage (sidescroll lane view)
        // ---------------------------------------------------------------

        void BuildRoomStage(Transform parent)
        {
            var root = UiFactory.CreateImage("RoomStage", parent, UiTheme.WithAlpha(Color.black, 0.35f));
            root.gameObject.AddComponent<LayoutElement>().preferredHeight = 200;

            _roomDepth = UiFactory.CreateText("RoomDepth", root.transform, "Entrance", 13, UiTheme.Bone, TextAlignmentOptions.Top);
            UiFactory.Anchor(_roomDepth.rectTransform, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1));
            _roomDepth.rectTransform.sizeDelta = new Vector2(0, 24);
            _roomDepth.rectTransform.anchoredPosition = new Vector2(0, -4);

            _doorState = UiFactory.CreateText("DoorState", root.transform, "🚪", 22, UiTheme.Muted, TextAlignmentOptions.MidlineLeft);
            UiFactory.Anchor(_doorState.rectTransform, new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0));
            _doorState.rectTransform.anchoredPosition = new Vector2(10, 10);
            _doorState.rectTransform.sizeDelta = new Vector2(30, 30);

            _floor = UiFactory.CreateRect("Floor", root.transform);
            UiFactory.Stretch(_floor);
            _floor.offsetMin = new Vector2(0, 30);
            _floor.offsetMax = new Vector2(0, -28);

            _heroToken = UiFactory.CreateText("HeroToken", _floor, "⚔", 30, UiTheme.Bone, TextAlignmentOptions.Center);
            UiFactory.Anchor(_heroToken.rectTransform, Vector2.zero, Vector2.zero, new Vector2(0.5f, 0f));
            _heroToken.rectTransform.sizeDelta = new Vector2(44, 44);
            _heroToken.gameObject.SetActive(false);

            _monsterToken = UiFactory.CreateText("MonsterToken", _floor, "👹", 30, UiTheme.EmberBright, TextAlignmentOptions.Center);
            UiFactory.Anchor(_monsterToken.rectTransform, Vector2.zero, Vector2.zero, new Vector2(0.5f, 0f));
            _monsterToken.rectTransform.sizeDelta = new Vector2(44, 44);
            _monsterToken.gameObject.SetActive(false);
        }

        void PlaceOnLane(RectTransform rt, float xPercent)
        {
            float w = _floor.rect.width;
            float h = _floor.rect.height;
            rt.anchoredPosition = new Vector2(w * (xPercent / 100f), h * (1f - LaneLayout.FloorY / 100f));
        }

        void PresentRoom(int index, DungeonSlotData slot)
        {
            var s = SaveSystem.State;
            _roomDepth.text = index == -1 ? "Entrance"
                : index == -2 ? "Throne Room"
                : $"Room {index + 1} / {Mathf.Max(1, s.SlotCount)}";
            if (_heroVisible) PlaceOnLane(_heroToken.rectTransform, LaneLayout.EntranceX);
        }

        void SetDoorOpen(bool open)
        {
            _doorState.text = open ? "🚪💨" : "🚪";
            _doorState.color = open ? UiTheme.Gold : UiTheme.Muted;
            if (open) StartCoroutine(WalkToEncounter());
        }

        IEnumerator WalkToEncounter()
        {
            float t = 0f;
            const float dur = 0.35f;
            Vector2 heroFrom = _heroToken.rectTransform.anchoredPosition;
            Vector2 heroTo = LanePos(LaneLayout.EncounterX);
            Vector2 monFrom = _monsterVisible ? _monsterToken.rectTransform.anchoredPosition : Vector2.zero;
            Vector2 monTo = LanePos(LaneLayout.EncounterX);
            while (t < dur)
            {
                t += Time.deltaTime;
                float f = Mathf.Clamp01(t / dur);
                if (_heroVisible) _heroToken.rectTransform.anchoredPosition = Vector2.Lerp(heroFrom, heroTo, f);
                if (_monsterVisible) _monsterToken.rectTransform.anchoredPosition = Vector2.Lerp(monFrom, monTo, f);
                yield return null;
            }
        }

        Vector2 LanePos(float xPercent)
        {
            float w = _floor.rect.width;
            float h = _floor.rect.height;
            return new Vector2(w * (xPercent / 100f), h * (1f - LaneLayout.FloorY / 100f));
        }

        void ShowHeroToken(Hero hero)
        {
            _heroVisible = true;
            _currentHero = hero;
            _heroToken.gameObject.SetActive(true);
            _heroToken.text = hero.Icon;
            PlaceOnLane(_heroToken.rectTransform, LaneLayout.EntranceX);
        }

        void HideHeroToken()
        {
            _heroVisible = false;
            _heroToken.gameObject.SetActive(false);
        }

        void ShowMonsterToken(string icon)
        {
            _monsterVisible = true;
            _monsterToken.gameObject.SetActive(true);
            _monsterToken.text = icon;
            PlaceOnLane(_monsterToken.rectTransform, LaneLayout.ExitX);
        }

        void HideMonsterToken()
        {
            _monsterVisible = false;
            _monsterToken.gameObject.SetActive(false);
        }

        void HeroWalkToExit()
        {
            if (_heroVisible) StartCoroutine(WalkHeroTo(LaneLayout.ExitX));
        }

        IEnumerator WalkHeroTo(float xPercent)
        {
            float t = 0f;
            const float dur = 0.3f;
            Vector2 from = _heroToken.rectTransform.anchoredPosition;
            Vector2 to = LanePos(xPercent);
            while (t < dur)
            {
                t += Time.deltaTime;
                _heroToken.rectTransform.anchoredPosition = Vector2.Lerp(from, to, Mathf.Clamp01(t / dur));
                yield return null;
            }
        }
    }
}
