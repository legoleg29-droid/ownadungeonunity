using UnityEngine;

namespace OwnADungeon.UI
{
    // Color tokens as Unity Color values (parsed from OwnADungeon.Data.Theme
    // hex strings), plus a couple of small UI-building helpers shared by
    // every runtime-built panel in this folder.
    public static class UiTheme
    {
        public static readonly Color Bg = Hex(Data.Theme.Bg);
        public static readonly Color Panel = Hex(Data.Theme.Panel);
        public static readonly Color PanelRaised = Hex(Data.Theme.PanelRaised);
        public static readonly Color Border = Hex(Data.Theme.Border);
        public static readonly Color BorderBright = Hex(Data.Theme.BorderBright);
        public static readonly Color Bone = Hex(Data.Theme.Bone);
        public static readonly Color Muted = Hex(Data.Theme.Muted);
        public static readonly Color MutedDim = Hex(Data.Theme.MutedDim);
        public static readonly Color Ember = Hex(Data.Theme.Ember);
        public static readonly Color EmberBright = Hex(Data.Theme.EmberBright);
        public static readonly Color Poison = Hex(Data.Theme.Poison);
        public static readonly Color PoisonBright = Hex(Data.Theme.PoisonBright);
        public static readonly Color Gold = Hex(Data.Theme.Gold);
        public static readonly Color Soul = Hex(Data.Theme.Soul);
        public static readonly Color Danger = Hex(Data.Theme.Danger);
        public static readonly Color Success = Hex(Data.Theme.Success);

        public static Color Hex(string hex)
        {
            ColorUtility.TryParseHtmlString(hex, out var c);
            return c;
        }

        public static Color WithAlpha(Color c, float a)
        {
            c.a = a;
            return c;
        }
    }
}
