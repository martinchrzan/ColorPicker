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

        public string ContentText
        {
            get { return (string)GetValue(ContentTextProperty); }
            set { SetValue(ContentTextProperty, value); }
        }
    }
}
