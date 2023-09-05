using ColorMeter.Helpers;
using ColorPicker.Helpers;
using ColorPicker.Settings;
using System;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Windows.Input;
using System.Windows.Threading;
using static ColorPicker.Win32Apis;

namespace ColorPicker.Mouse
{
    [Export(typeof(IMouseInfoProvider))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class MouseInfoProvider : IMouseInfoProvider
    {
        private const int MousePullInfoIntervalInMs = 1;
        private readonly DispatcherTimer _timer = new DispatcherTimer();
        private readonly MouseHook _mouseHook;
        private readonly AppStateHandler _appStateMonitor;
        private readonly IUserSettings _userSettings;
        private readonly IColorProvider _colorProvider;
        private System.Windows.Point _previousMousePosition = new System.Windows.Point(-1, 1);
        private Color _previousColor = Color.Transparent;
        private bool _colorFormatChanged = false;
        private bool _eventsSubscribed = false;

        [ImportingConstructor]
        public MouseInfoProvider(AppStateHandler appStateMonitor, IUserSettings userSettings, IColorProvider colorProvider)
        {
            _timer.Interval = TimeSpan.FromMilliseconds(MousePullInfoIntervalInMs);
            _timer.Tick += Timer_Tick;

            appStateMonitor.AppShown += AppStateMonitor_AppShown;
            appStateMonitor.AppClosed += AppStateMonitor_AppClosed;
            _mouseHook = new MouseHook();
            _appStateMonitor = appStateMonitor;
            _userSettings = userSettings;
            _colorProvider = colorProvider;
            _userSettings.SelectedColorFormat.PropertyChanged += SelectedColorFormat_PropertyChanged;
        }

        public event EventHandler<Color> MouseColorChanged;

        public event EventHandler<System.Windows.Point> MousePositionChanged;

        public event EventHandler<Tuple<System.Windows.Point, bool>> OnMouseWheel;

        public event MouseUpEventHandler OnLeftMouseDown;

        public event MouseUpEventHandler OnLeftMouseUp;

        public event MouseUpEventHandler OnRightMouseDown;

        public void StopMonitoring()
        {
            DisposeMouseHook();
        }

        public System.Windows.Point CurrentPosition
        {
            get
            {
                return _previousMousePosition;
            }
        }

        public void SetOriginalCursor()
        {
            if (_userSettings.ChangeCursor.Value)
            {
                CursorManager.RestoreOriginalCursors();
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

            var color = _colorProvider.GetPixelColor(mousePosition);
            if (_previousColor != color || _colorFormatChanged)
            {
                _previousColor = color;
                MouseColorChanged?.Invoke(this, color);
                _colorFormatChanged = false;
            }
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

        private void AppStateMonitor_AppShown(object sender, WindowType e)
        {
            UpdateMouseInfo();
            if (!_timer.IsEnabled)
            {
                _timer.Start();
            }

            if (!_eventsSubscribed)
            {
                _mouseHook.OnLeftMouseDown += MouseHook_OnLeftMouseDown;
                _mouseHook.OnLeftMouseUp += MouseHook_OnLeftMouseUp;
                _mouseHook.OnRightMouseDown += MouseHook_OnRightMouseDown;
                _mouseHook.OnMouseWheel += MouseHook_OnMouseWheel;
                _eventsSubscribed = true;
            }

            if (_userSettings.ChangeCursor.Value && e != WindowType.ColorMeter)
            {
                CursorManager.SetColorPickerCursor();
            }
        }

        private void MouseHook_OnLeftMouseUp(object sender, Point p)
        {
            OnLeftMouseUp?.Invoke(this, p);
            DisposeMouseHook();
        }

        private void MouseHook_OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta == 0)
            {
                return;
            }

            var zoomIn = e.Delta > 0;
            OnMouseWheel?.Invoke(this, new Tuple<System.Windows.Point, bool>(_previousMousePosition, zoomIn));
        }

        private void MouseHook_OnLeftMouseDown(object sender, Point p)
        {
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
            _mouseHook.OnLeftMouseUp -= MouseHook_OnLeftMouseUp;
            _mouseHook.OnRightMouseDown -= MouseHook_OnRightMouseDown;
            _mouseHook.OnMouseWheel -= MouseHook_OnMouseWheel;

            _eventsSubscribed = false;

            if (_userSettings.ChangeCursor.Value)
            {
                CursorManager.RestoreOriginalCursors();
            }
        }
    }
}
