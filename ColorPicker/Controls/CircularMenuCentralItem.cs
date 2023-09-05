using System.Windows;
using System.Windows.Controls;

namespace ColorPicker.Controls
{
    public class CircularMenuCentralItem : Button
    {
        static CircularMenuCentralItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CircularMenuCentralItem), new FrameworkPropertyMetadata(typeof(CircularMenuCentralItem)));
        }

        public static readonly DependencyProperty ContentTextProperty =
            DependencyProperty.Register("ContentText", typeof(string), typeof(CircularMenuCentralItem),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ColorNameProperty =
            DependencyProperty.Register("ColorName", typeof(string), typeof(CircularMenuCentralItem),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty IsColorNameVisibleProperty =
            DependencyProperty.Register("IsColorNameVisible", typeof(bool), typeof(CircularMenuCentralItem),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        public string ContentText
        {
            get { return (string)GetValue(ContentTextProperty); }
            set { SetValue(ContentTextProperty, value); }
        }

        public string ColorName
        {
            get { return (string)GetValue(ColorNameProperty); }
            set { SetValue(ColorNameProperty, value); }
        }

        public bool IsColorNameVisible
        {
            get { return (bool)GetValue(IsColorNameVisibleProperty); }
            set { SetValue(IsColorNameVisibleProperty, value); }
        }
    }
}
