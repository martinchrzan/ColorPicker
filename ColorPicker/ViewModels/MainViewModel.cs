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
            mouseInfoProvider.OnMouseDown += MouseInfoProvider_OnMouseDown;
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
            ColorString = ColorFormatHelper.ColorToString(color, _userSettings.SelectedColorFormat.Value);
            DisplayedColorBrush = new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
        }
        private void MouseInfoProvider_OnMouseDown(object sender, System.Drawing.Point p)
        {
            if (ColorString != null)
            {
                // nasty hack - sometimes clipboard can be in use and it will raise and exception
                for (int i = 0; i < 10; i++)
                {
                    try
                    {
                        Clipboard.SetText(ColorString.ToLowerInvariant());
                        break;
                    }
                    catch (COMException ex)
                    {
                        const uint CLIPBRD_E_CANT_OPEN = 0x800401D0; 
                        if ((uint)ex.ErrorCode != CLIPBRD_E_CANT_OPEN)
                        {
                            Logger.LogError("Failed to set text into clipboard", ex);
                        }
                    }
                    System.Threading.Thread.Sleep(10);
                }
            }

            _appStateHandler.HideColorPicker();
        }

        private void MouseInfoProvider_OnMouseWheel(object sender, Tuple<Point, bool> e)
        {
            _zoomWindowHelper.Zoom(e.Item1, e.Item2);
        }
    }
}
