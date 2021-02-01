using ColorPicker.Settings;
using System.ComponentModel.Composition;

namespace ColorPicker.Helpers
{
    [Export(typeof(ColorsHistoryWindowHelper))]
    public class ColorsHistoryWindowHelper
    {
        private IUserSettings _userSetting;
        private ColorsHistoryWindow _colorsHistoryWindow;

        [ImportingConstructor]
        public ColorsHistoryWindowHelper(IUserSettings userSettings)
        {
            _userSetting = userSettings;
        }

        public void ShowColorsHistory()
        {
            _colorsHistoryWindow = new ColorsHistoryWindow(_userSetting.ColorsHistory);
            _colorsHistoryWindow.Show();
        }

        public void HideColorsHistory()
        {
            _colorsHistoryWindow?.Close();
            _colorsHistoryWindow = null;
        }
    }
}
