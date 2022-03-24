
using System;
using System.Windows.Media;

namespace AwesomeOverlay.Core.Utilities.ColorUtilities
{
    public static class ColorExtensions
    {
        public static Color GetFromHex(string hex)
        {
            int a = 255;
            if (hex.Length == 9) {
                a = Convert.ToInt32(hex.Substring(1, 2), 16);
                hex = hex.Substring(3);
            }

            int r = Convert.ToInt32(hex.Substring(1, 2), 16);
            int g = Convert.ToInt32(hex.Substring(3, 2), 16);
            int b = Convert.ToInt32(hex.Substring(5, 2), 16);

            return Color.FromArgb((byte)a, (byte)r, (byte)g, (byte)b);
        }

        public static Color TurnHexIntoColor(this string hex)
        {
            return GetFromHex(hex);
        }
    }
}
