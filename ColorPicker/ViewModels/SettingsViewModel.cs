using ColorPicker.Common;
using ColorPicker.Helpers;
using ColorPicker.Settings;
using ColorPicker.ViewModelContracts;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Windows.Input;

namespace ColorPicker.ViewModels
{
    [Export(typeof(ISettingsViewModel))]
    public class SettingsViewModel : ViewModelBase, ISettingsViewModel
    {
        private readonly IUserSettings _userSettings;
        private bool _showingKeyboardCaptureOverlay = false;
        private string _shortcutPreview;
        private bool _checkingForUpdateInProgress;

        [ImportingConstructor]
        public SettingsViewModel(IUserSettings userSettings, AppUpdateManager appUpdateManager)
        {
            ChangeShortcutCommand = new RelayCommand(() =>
            {
                ShortCutPreview = ShortCut;
                ShowingKeyboardCaptureOverlay = true;
            });
            ConfirmShortcutCommand = new RelayCommand(() =>
            {
                ShortCut = ShortCutPreview;
                ShowingKeyboardCaptureOverlay = false;
            });

            CancelShortcutCommand = new RelayCommand(() =>
            {
                ShowingKeyboardCaptureOverlay = false;
            });

            CheckForUpdatesCommand = new RelayCommand(async () =>
            {
                CheckingForUpdateInProgress = true;
                if (await appUpdateManager.IsNewUpdateAvailable())
                {
                    await appUpdateManager.Update();
                }

                CheckingForUpdateInProgress = false;
            });

            _userSettings = userSettings;
            _userSettings.ActivationShortcut.PropertyChanged += (s, e) => { OnPropertyChanged(nameof(ShortCut)); };
        }

        public bool RunOnStartup
        {
            get
            {
                return _userSettings.RunOnStartup.Value;
            }
            set
            {
                // only set value if registry save successful 
                if(RegistryHelper.SetRunOnStartup(value))
                {
                    _userSettings.RunOnStartup.Value = value;
                    OnPropertyChanged();
                }
            }
        }
        public string ShortCut
        {
            get
            {
                return _userSettings.ActivationShortcut.Value;
            }
            private set
            {
                _userSettings.ActivationShortcut.Value = value;
                OnPropertyChanged();
            }
        }

        public string ShortCutPreview
        {
            get
            {
                return _shortcutPreview;
            }
            set
            {
                _shortcutPreview = value;
                OnPropertyChanged();
            }
        }

        public bool AutomaticUpdates
        {
            get
            {
                return _userSettings.AutomaticUpdates.Value;
            }
            set
            {
                _userSettings.AutomaticUpdates.Value = value;
                OnPropertyChanged();
            }
        }

        public bool ShowingKeyboardCaptureOverlay
        {
            get
            {
                return _showingKeyboardCaptureOverlay;
            }
            set
            {
                _showingKeyboardCaptureOverlay = value;
                OnPropertyChanged();
            }
        }

        public bool CheckingForUpdateInProgress
        {
            get
            {
                return _checkingForUpdateInProgress;
            }
            set
            {
                _checkingForUpdateInProgress = value;
                OnPropertyChanged();
            }
        }

        public string ApplicationVersion { get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); } }

        public ICommand ChangeShortcutCommand { get; }
        
        public ICommand CheckForUpdatesCommand { get; }
        
        public ICommand ConfirmShortcutCommand { get; }

        public ICommand CancelShortcutCommand { get; }
    }
}
