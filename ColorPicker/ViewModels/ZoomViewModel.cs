using ColorPicker.Common;
using ColorPicker.ViewModelContracts;
using System.ComponentModel.Composition;
using System.Windows.Media.Imaging;

namespace ColorPicker.ViewModels
{
    [Export(typeof(IZoomViewModel))]
    public class ZoomViewModel : ViewModelBase, IZoomViewModel
    {
        private BitmapSource _zoomArea;

        [ImportingConstructor]
        public ZoomViewModel()
        {
        }

        public BitmapSource ZoomArea
        {
            get
            {
                return _zoomArea;
            }
            set
            {
                _zoomArea = value;
                OnPropertyChanged();
            }
        }
    }
}
