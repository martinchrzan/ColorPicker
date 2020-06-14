using System;
using System.ComponentModel;

namespace ColorPicker.Settings
{
    public sealed class SettingItem<T> : INotifyPropertyChanged
    {
        private T _value;
        private readonly Action<T> _setValue;

        public SettingItem(T startValue, Action<T> setValue)
        {
            _value = startValue;
            _setValue = setValue;
        }

        public T Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                _setValue.Invoke(value);
                OnValueChanged(value);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnValueChanged(T newValue)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
        }
    }
}
