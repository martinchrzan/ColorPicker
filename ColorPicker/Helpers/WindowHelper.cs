using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace ColorPicker.Helpers
{
    internal static class WindowHelper
    {
        internal static void SetPositionAndSize(this Window window,
            double left,
            double top,
            double width,
            double height)
        {
            int pxLeft = 0, pxTop = 0;
            if (left != 0 || top != 0)
            {
                window.TransformToPixels(left, top, out pxLeft, out pxTop);
            }

            int pxWidth, pxHeight;
            window.TransformToPixels(width, height, out pxWidth, out pxHeight);

            var helper = new WindowInteropHelper(window);
            MoveWindow(helper.Handle, pxLeft, pxTop, pxWidth, pxHeight, true);
        }

        internal static void TransformToPixels(this Visual visual,
            double unitX,
            double unitY,
            out int pixelX,
            out int pixelY)
        {
            Matrix matrix;
            var source = PresentationSource.FromVisual(visual);
            if (source != null)
            {
                matrix = source.CompositionTarget.TransformToDevice;
            }
            else
            {
                using (var src = new HwndSource(new HwndSourceParameters()))
                {
                    matrix = src.CompositionTarget.TransformToDevice;
                }
            }

            pixelX = (int)(matrix.M11 * unitX);
            pixelY = (int)(matrix.M22 * unitY);
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, [MarshalAs(UnmanagedType.Bool)] bool bRepaint);
    }
}
