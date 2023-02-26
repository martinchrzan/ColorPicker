using System.Collections.Generic;
using System.Drawing;

namespace ColorPicker.Settings
{
    public enum ColorFormat { hex, rgb, hsl, hsv, vec4, rgb565, decimalBE, decimalLE};

    public interface IUserSettings
    {
        SettingItem<bool> RunOnStartup { get; }

        SettingItem<bool> AutomaticUpdates { get; }

        SettingItem<string> ActivationShortcut { get; }

        SettingItem<bool> ChangeCursor { get; }

        SettingItem<ColorFormat> SelectedColorFormat { get; }

        List<Color> ColorsHistory { get; }

        void AddColorIntoHistory(Color color);
    }
}
