using System;
using System.ComponentModel.Composition;

namespace ColorPicker.Settings
{
    [Export(typeof(IUserSettings))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class UserSettings : IUserSettings
    {
        [ImportingConstructor]
        public UserSettings()
        {
            var settings = Properties.Settings.Default;

            RunOnStartup = new SettingItem<bool>(settings.RunOnStartup, (currentValue) => { settings.RunOnStartup = currentValue; SaveSettings(); });
            AutomaticUpdates = new SettingItem<bool>(settings.AutomaticUpdates, (currentValue) => { settings.AutomaticUpdates = currentValue; SaveSettings(); });
            ActivationShortcut = new SettingItem<string>(settings.ActivationShortcut, (currentValue) => { settings.ActivationShortcut = currentValue; SaveSettings(); });
            ChangeCursor = new SettingItem<bool>(settings.ChangeCursorWhenPickingColor, (currentValue) => { settings.ChangeCursorWhenPickingColor = currentValue; SaveSettings(); });
            SelectedColorFormat = new SettingItem<ColorFormat>((ColorFormat)Enum.Parse(typeof(ColorFormat), settings.SelectedColorFormat, true), (currentValue) => { settings.SelectedColorFormat = currentValue.ToString(); SaveSettings(); });
        }

        public SettingItem<bool> RunOnStartup { get; }

        public SettingItem<bool> AutomaticUpdates { get; }

        public SettingItem<string> ActivationShortcut { get; }

        public SettingItem<bool> ChangeCursor { get; }

        public SettingItem<ColorFormat> SelectedColorFormat { get; }

        private void SaveSettings()
        {
            Properties.Settings.Default.Save();
        }
    }
}
