using System;
using System.ComponentModel.Composition;
using System.Windows.Threading;

namespace ColorPicker.Helpers
{
    [Export(typeof(IThrottledActionInvoker))]
    public sealed class ThrottledActionInvoker : IThrottledActionInvoker
    {
        private Action _actionToRun;

        private DispatcherTimer _timer;

        public ThrottledActionInvoker()
        {
            _timer = new DispatcherTimer();
            _timer.Tick += Timer_Tick;
        }

        public void ScheduleAction(Action action, int miliseconds)
        {
            if (_timer.IsEnabled)
            {
                _timer.Stop();
            }

            _actionToRun = action;
            _timer.Interval = new TimeSpan(0, 0, 0, 0, miliseconds);

            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _actionToRun.Invoke();
        }
    }
}
