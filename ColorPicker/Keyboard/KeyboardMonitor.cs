using ColorPicker.Helpers;
using ColorPicker.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ColorPicker.Keyboard
{
    [Export(typeof(KeyboardMonitor))]
    public class KeyboardMonitor
    {
        private readonly AppStateHandler _appStateHandler;
        private readonly IUserSettings _userSettings;

        private List<int> _currentlyPressedKeys = new List<int>();
        private List<int> _activationKeys = new List<int>();
        private GlobalKeyboardHook _keyboardHook;

        [ImportingConstructor]
        public KeyboardMonitor(AppStateHandler appStateHandler, IUserSettings userSettings)
        {
            _appStateHandler = appStateHandler;
            _userSettings = userSettings;
            _userSettings.ActivationShortcut.PropertyChanged += ActivationShortcut_PropertyChanged;
            SetActivationKeys();
        }

        public void Start()
        {
            _keyboardHook = new GlobalKeyboardHook();
            _keyboardHook.KeyboardPressed += Hook_KeyboardPressed;
        }

        private void SetActivationKeys()
        {
            _activationKeys.Clear();

            if (!string.IsNullOrEmpty(_userSettings.ActivationShortcut.Value))
            {
                var keys = _userSettings.ActivationShortcut.Value.Split('+');
                foreach (var key in keys)
                {
                    if (Enum.TryParse(key.Trim(), out Key parsedKey))
                    {
                        _activationKeys.Add(KeyInterop.VirtualKeyFromKey(parsedKey));
                    }
                }

                _activationKeys.Sort();
            }
        }

        private void ActivationShortcut_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            SetActivationKeys();
        }

        private void Hook_KeyboardPressed(object sender, GlobalKeyboardHookEventArgs e)
        {
            var virtualCode = e.KeyboardData.VirtualCode;
            if (e.KeyboardState == GlobalKeyboardHook.KeyboardState.KeyDown || e.KeyboardState == GlobalKeyboardHook.KeyboardState.SysKeyDown)
            {
                if (!_currentlyPressedKeys.Contains(virtualCode))
                {
                    _currentlyPressedKeys.Add(virtualCode);
                }
            }
            else if (e.KeyboardState == GlobalKeyboardHook.KeyboardState.KeyUp || e.KeyboardState == GlobalKeyboardHook.KeyboardState.SysKeyUp)
            {
                if (_currentlyPressedKeys.Contains(virtualCode))
                {
                    _currentlyPressedKeys.Remove(virtualCode);
                }
            }

            _currentlyPressedKeys.Sort();

            if(ArraysAreSame(_currentlyPressedKeys, _activationKeys))
            {
                _appStateHandler.ShowColorPicker();
            }
        }

        private bool ArraysAreSame(List<int> first, List<int> second)
        {
            if(first.Count != second.Count)
            {
                return false;
            }

            for(int i=0; i< first.Count; i++)
            {
                if(first[i] != second[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
