using ColorPicker.Helpers;
using ColorPicker.Settings;
using System;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Input;
using System.Windows.Threading;
using static ColorPicker.Win32Apis;

namespace ColorPicker.Mouse
{
    [Export(typeof(IMouseInfoProvider))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public sealed class MouseInfoProvider : IMouseInfoProvider, IDisposable
    {
        private const int MousePullInfoIntervalInMs = 10;
        private readonly DispatcherTimer _timer = new DispatcherTimer();
        private readonly MouseHook _mouseHook;
        private readonly IUserSettings _userSettings;
        
        private System.Windows.Point _previousMousePosition = new System.Windows.Point(-1, 1);
        private Color _previousColor = Color.Transparent;
        private bool _colorFormatChanged = false;
        
        private readonly Bitmap _bmp; // 1x1px-sized bitmap to capture desktop pixel via GDI.
        private readonly Graphics _bmpGraphics;

        [ImportingConstructor]
        public MouseInfoProvider(AppStateHandler appStateMonitor, IUserSettings userSettings)
        {
            _bmp = new Bitmap(1, 1, PixelFormat.Format32bppArgb);
            _bmpGraphics = Graphics.FromImage(_bmp);
            
            _mouseHook = new MouseHook();
            _userSettings = userSettings;
        
            _timer.Interval = TimeSpan.FromMilliseconds(MousePullInfoIntervalInMs);
            _timer.Tick += Timer_Tick;

            appStateMonitor.AppShown += AppStateMonitor_AppShown;
            appStateMonitor.AppClosed += AppStateMonitor_AppClosed;
            appStateMonitor.AppHidden += AppStateMonitor_AppClosed;
            
            _userSettings.SelectedColorFormat.PropertyChanged += SelectedColorFormat_PropertyChanged;
        }
        
        public void Dispose()
        {
            _bmpGraphics.Dispose();
            _bmp.Dispose();
        }

        public event EventHandler<Color> MouseColorChanged;

        public event EventHandler<System.Windows.Point> MousePositionChanged;

        public event EventHandler<Tuple<System.Windows.Point, bool>> OnMouseWheel;

        public event MouseUpEventHandler OnLeftMouseDown;

        public event MouseUpEventHandler OnRightMouseDown;

        public System.Windows.Point CurrentPosition
        {
            get
            {
                return _previousMousePosition;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateMouseInfo();
        }

        private void UpdateMouseInfo()
        {
            var mousePosition = GetCursorPosition();
            if (_previousMousePosition != mousePosition)
            {
                _previousMousePosition = mousePosition;
                MousePositionChanged?.Invoke(this, mousePosition);
            }

            var color = GetPixelColor(mousePosition);
            if (_previousColor != color || _colorFormatChanged)
            {
                _previousColor = color;
                MouseColorChanged?.Invoke(this, color);
                _colorFormatChanged = false;
            }
        }

        private static Color GetPixelColor(System.Windows.Point mousePosition)
        {
            int x = (int)mousePosition.X;
            int y = (int)mousePosition.Y;
            _bmpGraphics.CopyFromScreen(sourceX: x, sourceY: y, destinationX: 0, destinationY: 0, blockRegionSize: _bmp.Size, CopyPixelOperation.SourceCopy);

            return _bmp.GetPixel(0, 0);
        }

        private static System.Windows.Point GetCursorPosition()
        {
            GetCursorPos(out PointInter lpPoint);
            return (System.Windows.Point)lpPoint;
        }

        private void AppStateMonitor_AppClosed(object sender, EventArgs e)
        {
            DisposeMouseHook();
        }

        private void AppStateMonitor_AppShown(object sender, EventArgs e)
        {
            UpdateMouseInfo();
            if (!_timer.IsEnabled)
            {
                _timer.Start();
            }

            _mouseHook.OnLeftMouseDown += MouseHook_OnLeftMouseDown;
            _mouseHook.OnRightMouseDown += MouseHook_OnRightMouseDown;
            _mouseHook.OnMouseWheel += MouseHook_OnMouseWheel;

            if (_userSettings.ChangeCursor.Value)
            {
                CursorManager.SetColorPickerCursor();
            }
        }

        private void MouseHook_OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if(e.Delta == 0)
            {
                return;
            }

            var zoomIn = e.Delta > 0;
            OnMouseWheel?.Invoke(this, new Tuple<System.Windows.Point, bool>(_previousMousePosition, zoomIn));
        }

        private void MouseHook_OnLeftMouseDown(object sender, Point p)
        {
            DisposeMouseHook();
            OnLeftMouseDown?.Invoke(this, p);
        }

        private void MouseHook_OnRightMouseDown(object sender, Point p)
        {
            DisposeMouseHook();
            OnRightMouseDown?.Invoke(this, p);
        }

        private void SelectedColorFormat_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _colorFormatChanged = true;
        }

        private void DisposeMouseHook()
        {
            if (_timer.IsEnabled)
            {
                _timer.Stop();
            }
            
            _previousMousePosition = new System.Windows.Point(-1, 1);
            _mouseHook.OnLeftMouseDown -= MouseHook_OnLeftMouseDown;
            _mouseHook.OnRightMouseDown -= MouseHook_OnRightMouseDown;
            _mouseHook.OnMouseWheel -= MouseHook_OnMouseWheel;

            if (_userSettings.ChangeCursor.Value)
            {
                CursorManager.RestoreOriginalCursors();
            }
        }
    }
}
