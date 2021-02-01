using System.Collections.Generic;
using System.Drawing;
using Windows.ApplicationModel.Appointments.AppointmentsProvider;

namespace ColorPicker.Settings
{
    public enum ColorFormat { hex, rgb, hsl, hsv, vec4};

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
