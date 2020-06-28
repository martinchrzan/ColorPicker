using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorPicker.Settings
{
    public interface IUserSettings
    {
        SettingItem<bool> RunOnStartup { get; }

        SettingItem<bool> AutomaticUpdates { get; }

        SettingItem<string> ActivationShortcut { get; }

        SettingItem<bool> ChangeCursor { get; }
    }
}
