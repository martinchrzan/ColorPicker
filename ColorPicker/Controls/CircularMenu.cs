using ColorPicker.Helpers;
using ColorPicker.Settings;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace ColorPicker.Controls
{
    public class CircularMenu : ContentControl
    {
        private IUserSettings _userSettings;
        private ColorsHistoryWindowHelper _colorsHistoryWindowHelper;

        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(CircularMenu),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set
            {
                SetValue(IsOpenProperty, value);
            }
        }

        public static readonly DependencyProperty CentralItemProperty =
            DependencyProperty.Register("CentralItem", typeof(CircularMenuCentralItem), typeof(CircularMenu),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
        

        public CircularMenuCentralItem CentralItem
        {
            get { return (CircularMenuCentralItem)GetValue(CentralItemProperty); }
            set { SetValue(CentralItemProperty, value); }
        }

        public ObservableCollection<CircularMenuItem> Items
        {
            get { return (ObservableCollection<CircularMenuItem>)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        static CircularMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CircularMenu), new FrameworkPropertyMetadata(typeof(CircularMenu)));
        }

        public override void BeginInit()
        {
            Items = new ObservableCollection<CircularMenuItem>();
            Items.CollectionChanged += Items_CollectionChanged;
            base.BeginInit();
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                (e.NewItems[0] as CircularMenuItem).MouseEnter += CircularMenuItem_MouseEnter;
                (e.NewItems[0] as CircularMenuItem).PreviewMouseDown += CircularMenuItem_MouseDown;
                (e.NewItems[0] as CircularMenuItem).MouseLeave += CircularMenuItem_MouseLeave;
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                (e.NewItems[0] as CircularMenuItem).MouseEnter -= CircularMenuItem_MouseEnter;
                (e.NewItems[0] as CircularMenuItem).MouseLeave -= CircularMenuItem_MouseLeave; 
                (e.NewItems[0] as CircularMenuItem).PreviewMouseDown -= CircularMenuItem_MouseDown;
            }
            if(Items?.Count > 0)
            {
                CentralItem.ContentText = string.Empty;
            }
        }

        private void CircularMenuItem_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ClipboardHelper.CopyIntoClipboard(CentralItem.ContentText);
            if (_colorsHistoryWindowHelper == null)
            {
                _colorsHistoryWindowHelper = Bootstrapper.Container.GetExportedValue<ColorsHistoryWindowHelper>();
            }
            _colorsHistoryWindowHelper.HideColorsHistory();
        }

        private void CircularMenuItem_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            CentralItem.ContentText = string.Empty;
        }

        private void CircularMenuItem_MouseEnter(object sender, EventArgs e)
        {
            if(_userSettings == null)
            {
                _userSettings = Bootstrapper.Container.GetExportedValue<IUserSettings>();
            }
            
            var color = (sender as CircularMenuItem).Color;
            CentralItem.ContentText = ColorFormatHelper.ColorToString(color, _userSettings.SelectedColorFormat.Value);
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            for (int i = 0, count = Items.Count; i < count; i++)
            {
                Items[i].Index = i;
                Items[i].Count = count;
            }
            return base.ArrangeOverride(arrangeSize);
        }
    }
}
