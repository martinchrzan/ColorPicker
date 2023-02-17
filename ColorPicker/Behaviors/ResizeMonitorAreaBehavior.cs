using ColorMeter.Helpers;
using ColorPicker.Helpers;
using ColorPicker.Mouse;
using ColorPicker.Settings;
using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace ColorPicker.Behaviors
{
    public class ResizeMonitorAreaBehavior : Behavior<Window>
    {
        private const int MinWidth = 60;
        private const int BottomBorderHeight = 30;
        private IMouseInfoProvider _mouseInfoProvider;
        private IColorProvider _colorProvider;
        private IUserSettings _userSettings;
        private Point _corner;

        public static DependencyProperty CaptureAreaBorderProperty = DependencyProperty.Register("CaptureAreaBorder", typeof(Border), typeof(ResizeMonitorAreaBehavior));

        public static DependencyProperty ColorAreaBorderProperty = DependencyProperty.Register("ColorAreaBorder", typeof(Border), typeof(ResizeMonitorAreaBehavior));

        public static DependencyProperty BottomBorderRowProperty = DependencyProperty.Register("BottomBorderRow", typeof(RowDefinition), typeof(ResizeMonitorAreaBehavior));

        public static DependencyProperty ColorTextBlockProperty = DependencyProperty.Register("ColorTextBlock", typeof(TextBlock), typeof(ResizeMonitorAreaBehavior));
       
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

        public Border ColorAreaBorder
        {
            get
            {
                return (Border)GetValue(ColorAreaBorderProperty);
            }
            set
            {
                SetValue(ColorAreaBorderProperty, value);
            }
        }

        public RowDefinition BottomBorderRow
        {
            get
            {
                return (RowDefinition)GetValue(BottomBorderRowProperty);
            }
            set
            {
                SetValue(BottomBorderRowProperty, value);
            }
        }

        public TextBlock ColorTextBlock
        {
            get
            {
                return (TextBlock)GetValue(ColorTextBlockProperty);
            }
            set
            {
                SetValue(ColorTextBlockProperty, value);
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            _mouseInfoProvider = Bootstrapper.Container.GetExportedValue<IMouseInfoProvider>();
            _colorProvider = Bootstrapper.Container.GetExportedValue<IColorProvider>();
            _userSettings = Bootstrapper.Container.GetExportedValue<IUserSettings>();
            _mouseInfoProvider.OnLeftMouseDown += MouseInfoProvider_OnLeftMouseDown;

            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        private void setColor()
        {
            var dpi = MonitorResolutionHelper.GetCurrentMonitorDpi();
            var left = (AssociatedObject.Left + 3) * dpi.DpiScaleX;
            var top = (AssociatedObject.Top + 3) * dpi.DpiScaleX;
            var width = (CaptureAreaBorder.ActualWidth - 6) * dpi.DpiScaleX;
            var height = (CaptureAreaBorder.ActualHeight - 6) * dpi.DpiScaleX;

            var color = _colorProvider.GetAverageColor(new System.Drawing.Rectangle((int)left, (int)top, (int)width, (int)height));
            ColorTextBlock.Text = ColorFormatHelper.ColorToString(color, _userSettings.SelectedColorFormat.Value); ;
        }

        private bool _settingSize = false;
        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
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
                    ColorAreaBorder.Width = double.NaN;

                    AssociatedObject.SizeToContent = SizeToContent.WidthAndHeight;

                    BottomBorderRow.Height = new GridLength(BottomBorderHeight);

                    if(AssociatedObject.Width < MinWidth)
                    {
                        CaptureAreaBorder.Margin = new Thickness(0, 0, MinWidth - AssociatedObject.Width, CaptureAreaBorder.Margin.Bottom);
                        AssociatedObject.Width = MinWidth;
                    }

                    CaptureAreaBorder.HorizontalAlignment = HorizontalAlignment.Left;

                    var setHeight = new DoubleAnimation(0, BottomBorderHeight, new Duration(TimeSpan.FromMilliseconds(150)), FillBehavior.Stop);
                    setHeight.Completed += (s1, e1) =>
                    {
                        ColorAreaBorder.BeginAnimation(FrameworkElement.HeightProperty, null); ColorAreaBorder.Height = BottomBorderHeight;
                        setColor();
                    };

                    setHeight.EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseOut };

                    ColorAreaBorder.BeginAnimation(FrameworkElement.HeightProperty, setHeight, HandoffBehavior.Compose);
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

        private void MouseInfoProvider_OnLeftMouseDown(object sender, System.Drawing.Point p)
        {
            _corner = GetMousePositionScaled(_mouseInfoProvider.CurrentPosition);

            BottomBorderRow.Height = new GridLength(0);

            WindowHelper.SetPositionAndSize(AssociatedObject, _corner.X, _corner.Y, 0, 0);
            CaptureAreaBorder.Margin = new Thickness(0, 0, 0, 0);
            CaptureAreaBorder.HorizontalAlignment = HorizontalAlignment.Stretch;
            ColorAreaBorder.Height = 0;
      
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
                AssociatedObject.Width = 0; 
                AssociatedObject.Height = 0;
                BottomBorderRow.Height = new GridLength(0);
            }
        }

        private Point GetMousePositionScaled(Point mousePosition)
        {
            var dpi = MonitorResolutionHelper.GetCurrentMonitorDpi("MeterAreaMonitor");
            return new Point(mousePosition.X / dpi.DpiScaleX, mousePosition.Y / dpi.DpiScaleX);
        }
    }
}
