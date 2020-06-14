using ColorPicker.ViewModelContracts;
using System;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ColorPicker.Helpers
{
    [Export(typeof(ZoomWindowHelper))]
    public class ZoomWindowHelper
    {
        private const int ZoomFactor = 2;
        private const int BaseZoomImageSize = 50;
        private const int MaxZoomLevel = 4;
        private const int MinZoomLevel = 0;

        private int _currentZoomLevel = 0;
        private int _previousZoomLevel = 0;
        
        private readonly IZoomViewModel _zoomViewModel;
        private readonly AppStateHandler _appStateHandler;
        private ZoomWindow _zoomWindow;

        private double _lastLeft;
        private double _lastTop;

        [ImportingConstructor]
        public ZoomWindowHelper(IZoomViewModel zoomViewModel, AppStateHandler appStateHandler) 
        {
            _zoomViewModel = zoomViewModel;
            _appStateHandler = appStateHandler;
            _appStateHandler.AppClosed += AppStateHandler_AppClosed;
            _appStateHandler.AppHidden += AppStateHandler_AppClosed;
        }

        public void Zoom(System.Windows.Point position, bool zoomIn)
        {   
            if(zoomIn && _currentZoomLevel < MaxZoomLevel)
            {
                _previousZoomLevel = _currentZoomLevel;
                _currentZoomLevel++;
            }
            else if(!zoomIn && _currentZoomLevel > MinZoomLevel)
            {
                _previousZoomLevel = _currentZoomLevel;
                _currentZoomLevel--;
            }
            else
            {
                return;
            }

            SetZoomImage(position);
        }

        public void CloseZoomWindow()
        {
            _currentZoomLevel = 0;
            _previousZoomLevel = 0;
            HideZoomWindow();
        }

        private void SetZoomImage(System.Windows.Point point)
        {
            if (_currentZoomLevel == 0)
            {
                HideZoomWindow();
                return;
            }
            // we just started zooming, copy screen area
            if(_previousZoomLevel == 0 )
            {
                var x = (int)point.X - BaseZoomImageSize / 2;
                var y = (int)point.Y - BaseZoomImageSize / 2;
                var rect = new Rectangle(x, y, BaseZoomImageSize, BaseZoomImageSize);
                var bmp = new Bitmap(rect.Width, rect.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                var g = Graphics.FromImage(bmp);
                g.CopyFromScreen(rect.Left, rect.Top, 0, 0, bmp.Size, CopyPixelOperation.SourceCopy);

                var bitmapImage = BitmapToImageSource(bmp);

                _zoomViewModel.ZoomArea = bitmapImage;
            }
            else
            {
                var enlarge = (_currentZoomLevel - _previousZoomLevel) > 0 ? true : false;
                var currentZoomFactor = enlarge ? ZoomFactor : 1.0 / ZoomFactor;

                _zoomViewModel.ZoomArea = new TransformedBitmap(_zoomViewModel.ZoomArea, new ScaleTransform(currentZoomFactor, currentZoomFactor));
            }

            ShowZoomWindow((int)point.X, (int)point.Y);
        }

        private BitmapSource BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                return bitmapimage;
            }
        }

        private void HideZoomWindow()
        {
            if(_zoomWindow != null)
            {
                _zoomWindow.Hide();
            }
        }

        private void ShowZoomWindow(int x, int y)
        {
            if(_zoomWindow == null)
            {
                _zoomWindow = new ZoomWindow();
                _zoomWindow.Content = _zoomViewModel;
                _zoomWindow.Loaded += ZoomWindow_Loaded;
                _zoomWindow.IsVisibleChanged += ZoomWindow_IsVisibleChanged;
            }

            if (!_zoomWindow.IsVisible)
            {
                var dpi = MonitorResolutionHelper.GetCurrentMonitorDpi();
                var scaledX = x / dpi.DpiScaleX;
                var scaledY = y / dpi.DpiScaleY;
                _lastLeft = _zoomWindow.Left = scaledX - ((BaseZoomImageSize * Math.Pow(ZoomFactor, _currentZoomLevel - 1)) / 2);
                _lastTop = _zoomWindow.Top = scaledY - ((BaseZoomImageSize * Math.Pow(ZoomFactor, _currentZoomLevel - 1)) / 2);
                _zoomWindow.Show();
                
                _appStateHandler.SetTopMost();
            }
        }

        private void ZoomWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // need to set at this point again, to avoid issues moving between screens with different scaling
            if ((bool)e.NewValue)
            {
                _zoomWindow.Left = _lastLeft;
                _zoomWindow.Top = _lastTop;
            }
        }

        private void ZoomWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // need to call it again at load time, because it does was not dpi aware at the first time of Show() call
            _zoomWindow.Left = _lastLeft;
            _zoomWindow.Top = _lastTop;
        }

        private void AppStateHandler_AppClosed(object sender, EventArgs e)
        {
            HideZoomWindow();
        }
    }
}
