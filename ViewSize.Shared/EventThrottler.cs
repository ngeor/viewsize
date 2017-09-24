using System;
namespace CRLFLabs.ViewSize
{
    public class EventThrottler<T> where T : EventArgs
    {
        private DateTime _lastEvent = DateTime.MinValue;
        private readonly TimeSpan _threshold;
        private readonly EventHandler<T> _eventHandler;

        public EventThrottler(EventHandler<T> eventHandler, TimeSpan threshold)
        {
            _eventHandler = eventHandler;
            _threshold = threshold;
        }

        private bool ShouldFireEvent()
        {
            var now = DateTime.UtcNow;
            if (_lastEvent == DateTime.MinValue || now - _lastEvent > _threshold)
            {
                _lastEvent = now;
                return true;
            }

            return false;
        }

        public void ThrottledEventHandler(object sender, T args)
        {
            if (ShouldFireEvent())
            {
                _eventHandler(sender, args);
            }
        }

        public static EventHandler<T> Throttle(EventHandler<T> eventHandler, int milliseconds = 75)
        {
            return new EventThrottler<T>(eventHandler, TimeSpan.FromMilliseconds(milliseconds)).ThrottledEventHandler;
        }
    }
}
