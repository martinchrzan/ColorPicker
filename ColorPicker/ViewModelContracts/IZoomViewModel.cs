using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ColorPicker.ViewModelContracts
{
    public interface IZoomViewModel
    {
        BitmapSource ZoomArea { get; set; }
    }
}
