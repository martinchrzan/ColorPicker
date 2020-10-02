using ColorPicker.Settings;
using System;
using System.Globalization;

namespace ColorPicker.Helpers
{
    // helper class for converting color into various formats
    public static class ColorFormatHelper
    {
        public static string ColorToString(System.Drawing.Color c, ColorFormat format)
        {
            switch (format)
            {
                case ColorFormat.hex:
                    return ColorToHex(c);
                case ColorFormat.hsl:
                    return ColorToHsl(c);
                case ColorFormat.hsv:
                    return ColorToHsv(c);
                case ColorFormat.rgb:
                    return ColorToRgb(c);
                default:
                    return string.Empty;
            }
        }

        private static string ColorToHex(System.Drawing.Color c)
        {
            return "#" + c.R.ToString("X2", CultureInfo.InvariantCulture) + c.G.ToString("X2") + c.B.ToString("X2");
        }

        private static string ColorToRgb(System.Drawing.Color c)
        {
            return "rgb(" + c.R.ToString() + "," + c.G.ToString() + "," + c.B.ToString() + ")";
        }

        private static string ColorToHsl(System.Drawing.Color c)
        {
            var h = Math.Round(c.GetHue());
            var s = Math.Round(c.GetSaturation()*100);
            var l = Math.Round(c.GetBrightness()*100);
            return "hsl(" + h + ", " + s + "%, " + l + "%)";
        }

        private static string ColorToHsv(System.Drawing.Color c)
        {
            int max = Math.Max(c.R, Math.Max(c.G, c.B));
            int min = Math.Min(c.R, Math.Min(c.G, c.B));

            var h = Math.Round(c.GetHue());
            var s = (max == 0) ? 0 : 1d - (1d * min / max);
            var v = max / 255d;

            return "hsv(" + h + ", " + Math.Round(s*100) + ", " + Math.Round(v*100) + ")";
        }
    }
}
