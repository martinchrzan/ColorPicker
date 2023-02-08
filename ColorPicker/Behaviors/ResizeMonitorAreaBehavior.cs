using ColorMeter.Helpers;
using ColorPicker.Helpers;
using ColorPicker.Mouse;
using ColorPicker;
using Microsoft.Xaml.Behaviors;
using System;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace ColorPicker.Behaviors
{
    public class ResizeMonitorAreaBehavior : Behavior<Window>
    {
        private const int MinHeight = 60;
        private const int SideBorderWidth = 50;
        private IMouseInfoProvider _mouseInfoProvider;
        private IColorProvider _colorProvider;
        private DispatcherTimer _dispatcherTimer = new DispatcherTimer();
        private Point _corner;

        public static DependencyProperty CaptureAreaBorderProperty = DependencyProperty.Register("CaptureAreaBorder", typeof(Border), typeof(ResizeMonitorAreaBehavior));

        public static DependencyProperty RGBAreaBorderProperty = DependencyProperty.Register("RGBAreaBorder", typeof(Border), typeof(ResizeMonitorAreaBehavior));

        public static DependencyProperty SideBorderColumnProperty = DependencyProperty.Register("SideBorderColumn", typeof(ColumnDefinition), typeof(ResizeMonitorAreaBehavior));

        public static DependencyProperty RTextBlockProperty = DependencyProperty.Register("RTextBlock", typeof(TextBlock), typeof(ResizeMonitorAreaBehavior));
        public static DependencyProperty GTextBlockProperty = DependencyProperty.Register("GTextBlock", typeof(TextBlock), typeof(ResizeMonitorAreaBehavior));
        public static DependencyProperty BTextBlockProperty = DependencyProperty.Register("BTextBlock", typeof(TextBlock), typeof(ResizeMonitorAreaBehavior));

        public Border CaptureAreaBorder
        {
            get
            {
                return (Border)GetValue(CaptureAreaBorderProperty);
            }
            set
            {
                SetValue(CaptureAreaBorderProperty, value);
            }
        }

        public Border RGBAreaBorder
        {
            get
            {
                return (Border)GetValue(RGBAreaBorderProperty);
            }
            set
            {
                SetValue(RGBAreaBorderProperty, value);
            }
        }

        public ColumnDefinition SideBorderColumn
        {
            get
            {
                return (ColumnDefinition)GetValue(SideBorderColumnProperty);
            }
            set
            {
                SetValue(SideBorderColumnProperty, value);
            }
        }

        public TextBlock RTextBlock
        {
            get
            {
                return (TextBlock)GetValue(RTextBlockProperty);
            }
            set
            {
                SetValue(RTextBlockProperty, value);
            }
        }

        public TextBlock GTextBlock
        {
            get
            {
                return (TextBlock)GetValue(GTextBlockProperty);
            }
            set
            {
                SetValue(GTextBlockProperty, value);
            }
        }

        public TextBlock BTextBlock
        {
            get
            {
                return (TextBlock)GetValue(BTextBlockProperty);
            }
            set
            {
                SetValue(BTextBlockProperty, value);
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            _mouseInfoProvider = Bootstrapper.Container.GetExportedValue<IMouseInfoProvider>();
            _colorProvider = Bootstrapper.Container.GetExportedValue<IColorProvider>();
            _mouseInfoProvider.OnLeftMouseDown += MouseInfoProvider_OnLeftMouseDown;

            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        private bool _settingSize = false;
        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 50);
            _dispatcherTimer.Tick += DispatcherTimer_Tick;
            _corner = GetMousePositionScaled(new Point(Math.Round(_mouseInfoProvider.CurrentPosition.X), Math.Round(_mouseInfoProvider.CurrentPosition.Y)));

            WindowHelper.SetPositionAndSize(AssociatedObject, _corner.X, _corner.Y, 0, 0);
            _settingSize = true;

            _mouseInfoProvider.OnLeftMouseUp += (s, pos) =>
            {
                // minimum size is 3*3 (+borders)
                if (AssociatedObject.Height > 9 && AssociatedObject.Width > 9)
                {
                    CaptureAreaBorder.Width = AssociatedObject.Width;
                    CaptureAreaBorder.Height = AssociatedObject.Height;

                    AssociatedObject.SizeToContent = SizeToContent.WidthAndHeight;

                    //AssociatedObject.Width += SideBorderWidth;
                    SideBorderColumn.Width = new GridLength(SideBorderWidth);

                    if (AssociatedObject.Height < MinHeight)
                    {
                        CaptureAreaBorder.Margin = new Thickness(0, 0, 0, MinHeight - AssociatedObject.Height);
                        AssociatedObject.Height = MinHeight;
                    }

                    var setWidth = new DoubleAnimation(0, SideBorderWidth, new Duration(TimeSpan.FromMilliseconds(150)), FillBehavior.Stop);
                    setWidth.Completed += (s1, e1) =>
                    {
                        RGBAreaBorder.BeginAnimation(FrameworkElement.WidthProperty, null); RGBAreaBorder.Width = SideBorderWidth;
                        _dispatcherTimer.Start();
                    };

                    setWidth.EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseOut };

                    RGBAreaBorder.BeginAnimation(FrameworkElement.WidthProperty, setWidth, HandoffBehavior.Compose);
                }

                _settingSize = false;

            };

            _mouseInfoProvider.MousePositionChanged += (s, mousePosition) =>
            {
                if (_settingSize)
                {
                    var mousePositionScaled = GetMousePositionScaled(mousePosition);

                    var width = Math.Abs(_corner.X - Math.Round(mousePositionScaled.X));
                    var height = Math.Abs(_corner.Y - Math.Round(mousePositionScaled.Y));


                    var leftCornerX = _corner.X;
                    var leftCornerY = _corner.Y;

                    if (mousePositionScaled.X < _corner.X)
                    {
                        leftCornerX = mousePositionScaled.X;
                    }
                    if (mousePositionScaled.Y < _corner.Y)
                    {
                        leftCornerY = mousePositionScaled.Y;
                    }

                    WindowHelper.SetPositionAndSize(AssociatedObject, leftCornerX, leftCornerY, width, height);
                }
            };

            AssociatedObject.IsVisibleChanged += AssociatedObject_IsVisibleChanged;
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            var dpi = MonitorResolutionHelper.GetCurrentMonitorDpi();
            var left = (AssociatedObject.Left + 3) * dpi.DpiScaleX;
            var top = (AssociatedObject.Top + 3) * dpi.DpiScaleX;
            var width = (CaptureAreaBorder.ActualWidth - 6) * dpi.DpiScaleX;
            var height = (CaptureAreaBorder.ActualHeight - 6) * dpi.DpiScaleX;

            var color = _colorProvider.GetAverageColor(new System.Drawing.Rectangle((int)left, (int)top, (int)width, (int)height));
            RTextBlock.Text = "R: " + color.R;
            GTextBlock.Text = "G: " + color.G;
            BTextBlock.Text = "B: " + color.B;
        }

        private void MouseInfoProvider_OnLeftMouseDown(object sender, System.Drawing.Point p)
        {
            _corner = GetMousePositionScaled(_mouseInfoProvider.CurrentPosition);
            _dispatcherTimer.Stop();

            SideBorderColumn.Width = new GridLength(0);

            WindowHelper.SetPositionAndSize(AssociatedObject, _corner.X, _corner.Y, 0, 0);
            CaptureAreaBorder.Margin = new Thickness(0, 0, 0, 0);
            RGBAreaBorder.Width = 0;
            _settingSize = true;
        }

        private void AssociatedObject_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                var scaledMousePosition = GetMousePositionScaled(_mouseInfoProvider.CurrentPosition);

                WindowHelper.SetPositionAndSize(AssociatedObject, scaledMousePosition.X, scaledMousePosition.Y, 0, 0);
            }
            else
            {
                CaptureAreaBorder.Width = double.NaN;
                CaptureAreaBorder.Height = double.NaN;
                AssociatedObject.SizeToContent = SizeToContent.Manual;
                SideBorderColumn.Width = new GridLength(0);
                _dispatcherTimer.Stop();
            }
        }

        private Point GetMousePositionScaled(Point mousePosition)
        {
            var dpi = MonitorResolutionHelper.GetCurrentMonitorDpi("MeterAreaMonitor");
            return new Point(mousePosition.X / dpi.DpiScaleX, mousePosition.Y / dpi.DpiScaleX);
        }
    }
}
