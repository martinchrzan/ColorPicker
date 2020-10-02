using System.Windows.Media;

namespace ColorPicker.ViewModelContracts
{
    public interface IMainViewModel
    {
        string ColorString { get; }

        Brush DisplayedColorBrush { get; }
    }
}
