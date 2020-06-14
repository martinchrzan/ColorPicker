using Microsoft.Win32;
using System;
using System.Reflection;

namespace ColorPicker.Helpers
{
    static class RegistryHelper
    {
        private const string AppName = "ColorPicker";

        public static bool SetRunOnStartup(bool enabled)
        {
            try
            {
                RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

                if (enabled)
                {
                    rk.SetValue(AppName, Assembly.GetExecutingAssembly().Location);
                }
                else
                {
                    rk.DeleteValue(AppName, false);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Failed to set run on startup", ex);
                return false;
            }
            return true;
        }
    }
}
