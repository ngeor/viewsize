// <copyright file="EventThrottler.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;

namespace CRLFLabs.ViewSize
{
    public class EventThrottler<T> where T : EventArgs
    {
        private DateTime lastEvent = DateTime.MinValue;
        private readonly TimeSpan threshold;
        private readonly EventHandler<T> eventHandler;

        public EventThrottler(EventHandler<T> eventHandler, TimeSpan threshold)
        {
            this.eventHandler = eventHandler;
            this.threshold = threshold;
        }

        private bool ShouldFireEvent()
        {
            var now = DateTime.UtcNow;
            if (this.lastEvent == DateTime.MinValue || now - this.lastEvent > this.threshold)
            {
                this.lastEvent = now;
                return true;
            }

            return false;
        }

        public void ThrottledEventHandler(object sender, T args)
        {
            if (this.ShouldFireEvent())
            {
                this.eventHandler(sender, args);
            }
        }

        public static EventHandler<T> Throttle(EventHandler<T> eventHandler, int milliseconds = 75)
        {
            return new EventThrottler<T>(eventHandler, TimeSpan.FromMilliseconds(milliseconds)).ThrottledEventHandler;
        }
    }
}
