using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for ZoomWindow.xaml
    /// </summary>
    public partial class ZoomWindow : Window, INotifyPropertyChanged
    {
        private double _left;
        private double _top;
        public ZoomWindow()
        {

            InitializeComponent();
            DataContext = this;
        }

        public double DesiredLeft
        {
            get
            {
                return _left;
            }
            set
            {
                _left = value;
                NotifyPropertyChanged(nameof(DesiredLeft));
            }
        }

        public double DesiredTop
        {
            get
            {
                return _top;
            }
            set
            {
                _top = value;
                NotifyPropertyChanged(nameof(DesiredTop));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
