using ColorPicker.ViewModelContracts;
using System.ComponentModel.Composition;

namespace ColorPicker.Helpers
{
    [Export(typeof(SettingsWindowHelper))]
    public class SettingsWindowHelper
    {
        private SettingsWindow _settingsWindow;
        private readonly ISettingsViewModel _settingsViewModel;

        [ImportingConstructor]
        public SettingsWindowHelper(ISettingsViewModel settingsViewModel)
        {
            _settingsViewModel = settingsViewModel;
        }

        public void ShowSettings()
        {
            if (_settingsWindow == null || !_settingsWindow.IsVisible)
            {
                _settingsWindow = new SettingsWindow
                {
                    Content = _settingsViewModel
                };
                _settingsWindow.Show();
            }
        }
    }
}
