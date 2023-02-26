using ColorPicker.Settings;
using System;
using System.Drawing;
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
                case ColorFormat.vec4:
                    return ColorToVec4(c);
                case ColorFormat.rgb565:
                    return ColorToRgb565(c);
                case ColorFormat.decimalLE:
                    return ColorToDecimalLE(c);
                case ColorFormat.decimalBE:
                    return ColorToDecimalBE(c);
                default:
                    return string.Empty;
            }
        }

        // big-endian
        private static string ColorToDecimalBE(Color c)
        {
            return ((c.R * 265 * 265) + (c.G * 256) + c.B).ToString();
        }

        // little-endian
        private static string ColorToDecimalLE(Color c)
        {
            return ((c.B * 265 * 265) + (c.G * 256) + c.R).ToString();
        }

        private static string ColorToRgb565(Color c)
        {
            // Shift the red value to the leftmost 5 bits
            ushort r = (ushort)(c.R >> 3);
            // Shift the green value to the middle 6 bits
            ushort g = (ushort)(c.G >> 2);
            // Shift the blue value to the rightmost 5 bits
            ushort b = (ushort)(c.B >> 3);

            // Combine the shifted values into a single 16-bit value
            ushort rgb565 = (ushort)((r << 11) | (g << 5) | b);

            return "#" + rgb565.ToString("X2", CultureInfo.InvariantCulture);
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
            var s = Math.Round(c.GetSaturation() * 100);
            var l = Math.Round(c.GetBrightness() * 100);
            return "hsl(" + h + ", " + s + "%, " + l + "%)";
        }

        private static string ColorToHsv(System.Drawing.Color c)
        {
            int max = Math.Max(c.R, Math.Max(c.G, c.B));
            int min = Math.Min(c.R, Math.Min(c.G, c.B));

            var h = Math.Round(c.GetHue());
            var s = (max == 0) ? 0 : 1d - (1d * min / max);
            var v = max / 255d;

            return "hsv(" + h + ", " + Math.Round(s * 100) + ", " + Math.Round(v * 100) + ")";
        }

        private static string ColorToVec4(System.Drawing.Color c)
        {
            return string.Format("vec4({0}, {1}, {2}, 1)", Math.Round(c.R / 255f, 3), Math.Round(c.G / 255f, 3), Math.Round(c.B / 255f, 3));
        }
    }
}
