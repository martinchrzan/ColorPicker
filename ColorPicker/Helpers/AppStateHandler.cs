using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Documents;

namespace ColorPicker.Helpers
{
    public enum WindowType
    {
        ColorPicker,
        ZoomWindow,
        ColorMeter
    }

    [Export(typeof(AppStateHandler))]
    public class AppStateHandler
    {
        private readonly ColorsHistoryWindowHelper _colorsHistoryWindowHelper;
        private readonly List<WindowType> _currentlyShownApps = new List<WindowType>();

        private MeterAreaWindow _meter;
        private bool _meterAreaShown;
        
        [ImportingConstructor]
        public AppStateHandler(ColorsHistoryWindowHelper colorsHistoryWindowHelper) 
        {
            Application.Current.MainWindow.Closed += MainWindow_Closed;
            _colorsHistoryWindowHelper = colorsHistoryWindowHelper;
        }

        public event EventHandler<WindowType> AppShown;

        public event EventHandler<WindowType> AppHidden;

        public event EventHandler AppClosed;

        public void ShowColorPicker()
        {
            AddShownApp(WindowType.ColorPicker);
            AppShown?.Invoke(this, WindowType.ColorPicker);
            Application.Current.MainWindow.Opacity = 0;
            Application.Current.MainWindow.Visibility = Visibility.Visible;
        }

        public void HideColorPicker()
        {
            Application.Current.MainWindow.Opacity = 0;
            Application.Current.MainWindow.Visibility = Visibility.Collapsed;
            RemoveShownApp(WindowType.ColorPicker);
            AppHidden?.Invoke(this, WindowType.ColorPicker);
        }

        public bool IsMeterAreaShown
        {
            get
            {
                return _meterAreaShown;
            }
        }

        public List<WindowType> CurrentlyShownApps
        {
            get
            {
                return _currentlyShownApps;
            }
        }

        public void ShowMeterArea()
        {
            if (_meter == null)
            {
                _meter = new MeterAreaWindow();
            }
            _meter.Show();
            _meterAreaShown = true;
            AddShownApp(WindowType.ColorMeter);
            AppShown?.Invoke(this, WindowType.ColorMeter);
        }

        public void HideMeterArea()
        {
            if (_meter != null)
            {
                _meter.Hide();
                _meterAreaShown = false;
            }
            RemoveShownApp(WindowType.ColorMeter);
            AppHidden?.Invoke(this, WindowType.ColorMeter);
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

        private void AddShownApp(WindowType type)
        {
            if(!_currentlyShownApps.Contains(type))
            {
                _currentlyShownApps.Add(type);
            }
        }

        private void RemoveShownApp(WindowType type)
        {
            if (_currentlyShownApps.Contains(type))
            {
                _currentlyShownApps.Remove(type);
            }
        }
    }
}
