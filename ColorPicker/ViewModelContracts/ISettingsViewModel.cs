using ColorPicker.Settings;
using System.Windows.Input;

namespace ColorPicker.ViewModelContracts
{
    public interface ISettingsViewModel
    {
        bool RunOnStartup { get; set; }

        bool AutomaticUpdates { get; set; }

        bool ShowingKeyboardCaptureOverlay { get; set; }

        bool ChangeCursorWhenPickingColor { get; set; }

        bool ShowColorName { get; set; }

        string ShortCut { get; }

        string ShortCutPreview { get; set; }

        string ApplicationVersion { get; }

        ColorFormat SelectedColorFormat { get; }

        bool CheckingForUpdateInProgress { get; }

        ICommand ChangeShortcutCommand { get; }

        ICommand CheckForUpdatesCommand { get; }

        ICommand ConfirmShortcutCommand { get; }

        ICommand CancelShortcutCommand { get; }
    }
}
