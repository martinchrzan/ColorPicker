using System;

namespace ColorPicker.Helpers
{
    public interface IThrottledActionInvoker
    {
        void ScheduleAction(Action action, int miliseconds);
    }
}
