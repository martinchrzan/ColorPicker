using System;
using System.ComponentModel.Composition;
using System.Windows;

namespace ColorPicker.Helpers
{
    [Export(typeof(AppStateHandler))]
    public class AppStateHandler
    {
        private readonly ColorsHistoryWindowHelper _colorsHistoryWindowHelper;

        [ImportingConstructor]
        public AppStateHandler(ColorsHistoryWindowHelper colorsHistoryWindowHelper) 
        {
            Application.Current.MainWindow.Closed += MainWindow_Closed;
            _colorsHistoryWindowHelper = colorsHistoryWindowHelper;
        }

        public event EventHandler AppShown;

        public event EventHandler AppHidden;

        public event EventHandler AppClosed;

        public void ShowColorPicker()
        {
            AppShown?.Invoke(this, EventArgs.Empty);
            Application.Current.MainWindow.Opacity = 0;
            Application.Current.MainWindow.Visibility = Visibility.Visible;
        }

        public void HideColorPicker()
        {
            Application.Current.MainWindow.Opacity = 0;
            Application.Current.MainWindow.Visibility = Visibility.Collapsed;
            AppHidden?.Invoke(this, EventArgs.Empty);
        }

        public void ShowColorHistory()
        {
            HideColorPicker();
            _colorsHistoryWindowHelper.ShowColorsHistory();
        }

        public void SetTopMost()
        {
            Application.Current.MainWindow.Topmost = false;
            Application.Current.MainWindow.Topmost = true;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            AppClosed?.Invoke(this, EventArgs.Empty);
        }
    }
}
