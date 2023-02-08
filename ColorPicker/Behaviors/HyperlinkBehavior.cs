using ColorPicker.Helpers;
using Microsoft.Xaml.Behaviors;
using System;
using System.Diagnostics;
using System.Windows.Documents;

namespace ColorPicker.Behaviors
{
    public class HyperlinkBehavior : Behavior<Hyperlink>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.RequestNavigate += AssociatedObject_RequestNavigate;
        }

        private void AssociatedObject_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Logger.LogError("Failed to navigate into project website", ex);
            }
        }
    }
}
