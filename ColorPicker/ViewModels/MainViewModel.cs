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
        private string _hexColor;
        private string _rgbColor;
        private Brush _colorBrush;
        private readonly ZoomWindowHelper _zoomWindowHelper;
        private readonly AppStateHandler _appStateHandler;

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

        public string HexColor
        {
            get
            {
                return _hexColor;
            }
            private set
            {
                _hexColor = value;
                OnPropertyChanged();
            }
        }

        public string RgbColor
        {
            get
            {
                return _rgbColor;
            }
            private set
            {
                _rgbColor = value;
                OnPropertyChanged();
            }
        }

        public Brush ColorBrush
        {
            get
            {
                return _colorBrush;
            }
            private set
            {
                _colorBrush = value;
                OnPropertyChanged();
            }
        }

        private void Mouse_ColorChanged(object sender, System.Drawing.Color color)
        {
            HexColor = ColorToHex(color);
            RgbColor = ColorToRGB(color);
            ColorBrush = new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
        }
        private void MouseInfoProvider_OnMouseDown(object sender, System.Drawing.Point p)
        {
            if (HexColor != null)
            {
                // nasty hack - sometimes clipboard can be in use and it will raise and exception
                for (int i = 0; i < 10; i++)
                {
                    try
                    {
                        Clipboard.SetText(HexColor);
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

        private static string ColorToHex(System.Drawing.Color c)
        {
            return "#" + c.R.ToString("X2", CultureInfo.InvariantCulture) + c.G.ToString("X2") + c.B.ToString("X2");
        }

        private static string ColorToRGB(System.Drawing.Color c)
        {
            return "RGB(" + c.R.ToString() + "," + c.G.ToString() + "," + c.B.ToString() + ")";
        }
    }
}
