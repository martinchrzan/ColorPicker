using ColorPicker.Settings;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace ColorPicker.Behaviors
{
    public class SetShortcutBehavior : Behavior<UserControl>
    {
        public bool MonitorKeys
        {
            get { return (bool)GetValue(MonitorKeysProperty); }
            set { SetValue(MonitorKeysProperty, value); }
        }

        public static DependencyProperty MonitorKeysProperty = DependencyProperty.Register(
            "MonitorKeys", typeof(bool), typeof(SetShortcutBehavior));

        public string ShortCutPreview
        {
            get { return (string)GetValue(ShortCutPreviewProperty); }
            set { SetValue(ShortCutPreviewProperty, value); }
        }

        public static DependencyProperty ShortCutPreviewProperty = DependencyProperty.Register(
            "ShortCutPreview", typeof(string), typeof(SetShortcutBehavior));

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            AssociatedObject.PreviewKeyDown += AssociatedObject_PreviewKeyDown;
        }

        private void AssociatedObject_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (MonitorKeys)
            {
                var pressedKeys = new List<string>();

                if (System.Windows.Input.Keyboard.IsKeyDown(Key.LeftShift))
                {
                    pressedKeys.Add("LeftShift");
                }
                if (System.Windows.Input.Keyboard.IsKeyDown(Key.RightShift))
                {
                    pressedKeys.Add("RightShift");
                }

                if (System.Windows.Input.Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    pressedKeys.Add("LeftCtrl");
                }
                if (System.Windows.Input.Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    pressedKeys.Add("RightCtrl");
                }

                if (System.Windows.Input.Keyboard.IsKeyDown(Key.LeftAlt))
                {
                    pressedKeys.Add("LeftAlt");
                }
                if (System.Windows.Input.Keyboard.IsKeyDown(Key.RightAlt))
                {
                    pressedKeys.Add("RightAlt");
                }

                // ignore modifiers, we captured them above already
                if (e.Key != Key.LeftShift && e.Key != Key.RightShift &&
                    e.Key != Key.LeftCtrl && e.Key != Key.RightCtrl &&
                    e.Key != Key.LeftAlt && e.Key != Key.RightAlt &&
                    e.Key != Key.System)
                {
                    pressedKeys.Add(e.Key.ToString());
                }

                var allKeys = string.Join(" + ", pressedKeys);
                ShortCutPreview = allKeys;
            }
        }
    }
}
