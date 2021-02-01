using System.Runtime.InteropServices;
using System.Windows;

namespace ColorPicker.Helpers
{
    public static class ClipboardHelper
    {
        public static void CopyIntoClipboard(string value)
        {
            // nasty hack - sometimes clipboard can be in use and it will raise and exception
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    Clipboard.SetDataObject(value.ToLowerInvariant());
                    break;
                }
                catch (COMException ex)
                {
                    const uint CLIPBRD_E_CANT_OPEN = 0x800401D0;
                    if ((uint)ex.ErrorCode != CLIPBRD_E_CANT_OPEN)
                    {
                        Logger.LogError("Failed to set text into clipboard", ex);
                    }
                }
                System.Threading.Thread.Sleep(10);
            }
        }
    }
}
