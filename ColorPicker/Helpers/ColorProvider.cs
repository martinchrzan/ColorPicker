using System;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Effects;

namespace ColorMeter.Helpers
{
    [Export(typeof(IColorProvider))]
    class ColorProvider : IColorProvider
    {
        private Bitmap _bmp;
        private Graphics _bmpGraphics;

        public ColorProvider() {
            _bmp = new Bitmap(1, 1, PixelFormat.Format32bppArgb);
            _bmpGraphics = Graphics.FromImage(_bmp);
        }

        public Color GetAverageColor(Rectangle area)
        {
            using (var bmp = new Bitmap(area.Width, area.Height, PixelFormat.Format32bppArgb))
            {
                using (var g = Graphics.FromImage(bmp))
                {
                    g.CopyFromScreen(area.Left, area.Top, 0, 0, bmp.Size, CopyPixelOperation.SourceCopy);

                    ulong r = 0;
                    ulong gg = 0;
                    ulong b = 0;
                    ulong pixels = 0;
                    for (int i = 0; i < area.Width; i++)
                    {
                        for (int j = 0; j < area.Height; j++)
                        {
                            var pixel = bmp.GetPixel(i, j);
                            r += pixel.R;
                            gg += pixel.G;
                            b += pixel.B;
                            pixels++;
                        }
                    }

                    return Color.FromArgb((int)Math.Round((double)(r / pixels)), (int)Math.Round((double)(gg / pixels)), (int)Math.Round((double)(b / pixels)));
                }
            }
        }

        public Color GetPixelColor(System.Windows.Point pixelPosition)
        {
            int x = (int)pixelPosition.X;
            int y = (int)pixelPosition.Y;   
            _bmpGraphics.CopyFromScreen(sourceX: x, sourceY: y, destinationX: 0, destinationY: 0, blockRegionSize: _bmp.Size, CopyPixelOperation.SourceCopy);
            return _bmp.GetPixel(0, 0);
        }
    }
}
