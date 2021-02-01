using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ColorPicker
{
    /// <summary>
    /// Interaction logic for ColorsHistoryWindow.xaml
    /// </summary>
    public partial class ColorsHistoryWindow : Window
    {
        public ColorsHistoryWindow(List<System.Drawing.Color> colorsHistory)
        {
            InitializeComponent();
            foreach(var color in colorsHistory)
            {
                circularMenu.Items.Add(new Controls.CircularMenuItem() { Color = color });
            }

            Loaded += ColorsHistoryWindow_Loaded;
        }

        private void ColorsHistoryWindow_Loaded(object sender, RoutedEventArgs e)
        {
            circularMenu.IsOpen = true;
        }
    }
}
