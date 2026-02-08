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
                case ColorFormat.hct:
                    return ColorToHct(c);
                case ColorFormat.srgbLinear:
                    return ColorToSrgbLinear(c);
                case ColorFormat.oklab:
                    return ColorToOklab(c);
                case ColorFormat.oklch:
                    return ColorToOklch(c);
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

        private static string ColorToHct(System.Drawing.Color c)
        {
            var hct = RgbToHct(c.R, c.G, c.B);
            return "hct(" + Math.Round(hct.H) + ", " + Math.Round(hct.C) + ", " + Math.Round(hct.T) + ")";
        }

        private static string ColorToSrgbLinear(System.Drawing.Color c)
        {
            double rLinear = Linearize(c.R / 255.0);
            double gLinear = Linearize(c.G / 255.0);
            double bLinear = Linearize(c.B / 255.0);
            return string.Format("srgb-linear({0}, {1}, {2})", Math.Round(rLinear, 3), Math.Round(gLinear, 3), Math.Round(bLinear, 3));
        }

        private static string ColorToOklab(System.Drawing.Color c)
        {
            var oklab = RgbToOklab(c.R, c.G, c.B);
            return "oklab(" + Math.Round(oklab.L, 3) + " " + Math.Round(oklab.A, 3) + " " + Math.Round(oklab.B, 3) + ")";
        }

        private static string ColorToOklch(System.Drawing.Color c)
        {
            var oklch = RgbToOklch(c.R, c.G, c.B);
            return "oklch(" + Math.Round(oklch.L, 3) + " " + Math.Round(oklch.C, 3) + " " + Math.Round(oklch.H) + ")";
        }

        private struct Hct
        {
            public double H { get; set; }
            public double C { get; set; }
            public double T { get; set; }
        }

        private struct Oklab
        {
            public double L { get; set; }
            public double A { get; set; }
            public double B { get; set; }
        }

        private struct Oklch
        {
            public double L { get; set; }
            public double C { get; set; }
            public double H { get; set; }
        }

        private static Oklab RgbToOklab(int r, int g, int b)
        {
            // Convert RGB to linear sRGB
            double rLinear = Linearize(r / 255.0);
            double gLinear = Linearize(g / 255.0);
            double bLinear = Linearize(b / 255.0);

            // Convert linear sRGB to LMS
            double l = 0.4122214708 * rLinear + 0.5310886647 * gLinear + 0.0514459929 * bLinear;
            double m = 0.2119034982 * rLinear + 0.6807612419 * gLinear + 0.1073790969 * bLinear;
            double s = 0.0883024619 * rLinear + 0.0853627145 * gLinear + 0.8301696423 * bLinear;

            // Convert LMS to Oklab
            double l_ = Math.Pow(l, 1.0 / 3.0);
            double m_ = Math.Pow(m, 1.0 / 3.0);
            double s_ = Math.Pow(s, 1.0 / 3.0);

            return new Oklab
            {
                L = 0.2104542553 * l_ + 0.7936177850 * m_ - 0.0040720468 * s_,
                A = 1.9779984951 * l_ - 2.4285922050 * m_ + 0.4505937099 * s_,
                B = 0.0259040371 * l_ + 0.7827717662 * m_ - 0.8086757660 * s_
            };
        }

        private static Oklch RgbToOklch(int r, int g, int b)
        {
            var oklab = RgbToOklab(r, g, b);
            double c = Math.Sqrt(oklab.A * oklab.A + oklab.B * oklab.B);
            double h = Math.Atan2(oklab.B, oklab.A) * (180.0 / Math.PI);
            if (h < 0)
                h += 360;

            return new Oklch
            {
                L = oklab.L,
                C = c,
                H = h
            };
        }

        private static Hct RgbToHct(int r, int g, int b)
        {
            // Convert RGB to linear sRGB
            double rLinear = Linearize(r / 255.0);
            double gLinear = Linearize(g / 255.0);
            double bLinear = Linearize(b / 255.0);

            // Convert linear sRGB to XYZ
            double x = (0.4124564 * rLinear + 0.3575761 * gLinear + 0.1804375 * bLinear);
            double y = (0.2126729 * rLinear + 0.7151522 * gLinear + 0.0721750 * bLinear);
            double z = (0.0193339 * rLinear + 0.1191920 * gLinear + 0.9503041 * bLinear);

            // Convert XYZ to LAB
            double l = 116.0 * LabF(y / 1.0) - 16.0;
            double a = 500.0 * (LabF(x / 0.95047) - LabF(y / 1.0));
            double b_ = 200.0 * (LabF(y / 1.0) - LabF(z / 1.08883));

            // Convert LAB to LCH (Lch)
            double c = Math.Sqrt(a * a + b_ * b_);
            double h = Math.Atan2(b_, a) * (180.0 / Math.PI);
            if (h < 0)
                h += 360;

            // In HCT, T (tone) is derived from L (lightness)
            double t = l;

            return new Hct { H = h, C = c, T = t };
        }

        private static double Linearize(double value)
        {
            if (value <= 0.04045)
                return value / 12.92;
            return Math.Pow((value + 0.055) / 1.055, 2.4);
        }

        private static double LabF(double t)
        {
            double delta = 6.0 / 29.0;
            if (t > delta * delta * delta)
                return Math.Pow(t, 1.0 / 3.0);
            return t / (3.0 * delta * delta) + 4.0 / 29.0;
        }
    }
}
