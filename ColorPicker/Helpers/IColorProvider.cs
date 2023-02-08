using System.Drawing;

namespace ColorMeter.Helpers
{
    public interface IColorProvider
    {
        Color GetPixelColor(System.Windows.Point pixelPosition);

        Color GetAverageColor(Rectangle area);
    }
}
