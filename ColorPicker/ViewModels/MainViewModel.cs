using ColorPicker.Common;
using ColorPicker.Helpers;
using ColorPicker.Keyboard;
using ColorPicker.Mouse;
using ColorPicker.Settings;
using ColorPicker.ViewModelContracts;
using System;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace ColorPicker.ViewModels
{
    [Export(typeof(IMainViewModel))]
    public class MainViewModel : ViewModelBase, IMainViewModel
    {
        private string _colorString;
        private Brush _displayedColorBrush;
        private readonly ZoomWindowHelper _zoomWindowHelper;
        private readonly AppStateHandler _appStateHandler;
        private readonly IUserSettings _userSettings;
        private System.Drawing.Color _currentColor;

        [ImportingConstructor]
        public MainViewModel(
            IMouseInfoProvider mouseInfoProvider, 
            ZoomWindowHelper zoomWindowHelper, 
            AppStateHandler appStateHandler, 
            KeyboardMonitor keyboardMonitor, 
            AppUpdateManager appUpdateManager,
            IUserSettings userSettings)
        {
            _zoomWindowHelper = zoomWindowHelper;
            _appStateHandler = appStateHandler;
            _userSettings = userSettings;
            mouseInfoProvider.MouseColorChanged += Mouse_ColorChanged;
            mouseInfoProvider.OnLeftMouseDown += MouseInfoProvider_OnLeftMouseDown;
            mouseInfoProvider.OnRightMouseDown += MouseInfoProvider_OnRightMouseDown;
            mouseInfoProvider.OnMouseWheel += MouseInfoProvider_OnMouseWheel;

            keyboardMonitor.Start();
            
            #if !DEBUG
            CheckForUpdates(appUpdateManager, userSettings);
            #endif
        }

        private static void CheckForUpdates(AppUpdateManager appUpdateManager, IUserSettings userSettings)
        {
            if (userSettings.AutomaticUpdates.Value)
            {
                Task.Run(async () =>
                {
                    // do not start it immediately after the app start
                    await Task.Delay(TimeSpan.FromSeconds(5));
                    if (await appUpdateManager.IsNewUpdateAvailable())
                    {
                        await appUpdateManager.Update();
                    }
                });
            }
        }

        public string ColorString
        {
            get
            {
                return _colorString;
            }
            set
            {
                _colorString = value;
                OnPropertyChanged();
            }
        }

        public Brush DisplayedColorBrush
        {
            get
            {
                return _displayedColorBrush;
            }
            private set
            {
                _displayedColorBrush = value;
                OnPropertyChanged();
            }
        }

        private void Mouse_ColorChanged(object sender, System.Drawing.Color color)
        {
            _currentColor = color;
            ColorString = ColorFormatHelper.ColorToString(color, _userSettings.SelectedColorFormat.Value);
            DisplayedColorBrush = new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
        }
        private void MouseInfoProvider_OnLeftMouseDown(object sender, System.Drawing.Point p)
        {
            if (ColorString != null)
            {
                ClipboardHelper.CopyIntoClipboard(ColorString);
                _userSettings.AddColorIntoHistory(_currentColor);
            }

            _appStateHandler.HideColorPicker();
        }

        private void MouseInfoProvider_OnRightMouseDown(object sender, System.Drawing.Point p)
        {
            _appStateHandler.ShowColorHistory();
        }

        private void MouseInfoProvider_OnMouseWheel(object sender, Tuple<Point, bool> e)
        {
            _zoomWindowHelper.Zoom(e.Item1, e.Item2);
        }
    }
}
