using ColorPicker.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;

namespace ColorPicker.Settings
{
    [Export(typeof(IUserSettings))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class UserSettings : IUserSettings
    {
        private const int ColorsHistoryLimit = 10;
        private const string ColorsSeparator = ";";

        [ImportingConstructor]
        public UserSettings()
        {
            var settings = Properties.Settings.Default;

            RunOnStartup = new SettingItem<bool>(settings.RunOnStartup, (currentValue) => { settings.RunOnStartup = currentValue; SaveSettings(); });
            AutomaticUpdates = new SettingItem<bool>(settings.AutomaticUpdates, (currentValue) => { settings.AutomaticUpdates = currentValue; SaveSettings(); });
            ActivationShortcut = new SettingItem<string>(settings.ActivationShortcut, (currentValue) => { settings.ActivationShortcut = currentValue; SaveSettings(); });
            ChangeCursor = new SettingItem<bool>(settings.ChangeCursorWhenPickingColor, (currentValue) => { settings.ChangeCursorWhenPickingColor = currentValue; SaveSettings(); });
            ShowColorName = new SettingItem<bool>(settings.ShowColorName, (currentValue) => { settings.ShowColorName = currentValue; SaveSettings(); });
            SelectedColorFormat = new SettingItem<ColorFormat>((ColorFormat)Enum.Parse(typeof(ColorFormat), settings.SelectedColorFormat, true), (currentValue) => { settings.SelectedColorFormat = currentValue.ToString(); SaveSettings(); });
            LoadColorsHistory();
        }

        public SettingItem<bool> RunOnStartup { get; }

        public SettingItem<bool> AutomaticUpdates { get; }

        public SettingItem<string> ActivationShortcut { get; }

        public SettingItem<bool> ChangeCursor { get; }

        public SettingItem<bool> ShowColorName { get; }

        public SettingItem<ColorFormat> SelectedColorFormat { get; }

        public List<Color> ColorsHistory { get; } = new List<Color>();

        public void AddColorIntoHistory(Color color)
        {
            if (ColorsHistory.Count > ColorsHistoryLimit - 1)
            {
                ColorsHistory.RemoveAt(ColorsHistoryLimit - 1);
            }
            ColorsHistory.Insert(0, color);
            Properties.Settings.Default.ColorsHistory = string.Join(ColorsSeparator, ColorsHistory.ConvertAll(c => ColorFormatHelper.ColorToString(c, ColorFormat.hex)));
            SaveSettings();
        }

        private void LoadColorsHistory()
        {
            if (!string.IsNullOrEmpty(Properties.Settings.Default.ColorsHistory))
            {
                foreach (var color in Properties.Settings.Default.ColorsHistory.Split(';'))
                {
                    ColorsHistory.Add(ColorTranslator.FromHtml(color));
                }
            }
        }

        private void SaveSettings()
        {
            Properties.Settings.Default.Save();
        }
    }
}
