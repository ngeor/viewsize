﻿// <copyright file="EventThrottler.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;

namespace CRLFLabs.ViewSize
{
    public class EventThrottler<T>
        where T : EventArgs
    {
        private readonly TimeSpan threshold;
        private readonly EventHandler<T> eventHandler;
        private DateTime lastEvent = DateTime.MinValue;

        public EventThrottler(EventHandler<T> eventHandler, TimeSpan threshold)
        {
            this.eventHandler = eventHandler;
            this.threshold = threshold;
        }

        public static EventHandler<T> Throttle(EventHandler<T> eventHandler, int milliseconds = 75)
        {
            return new EventThrottler<T>(eventHandler, TimeSpan.FromMilliseconds(milliseconds)).ThrottledEventHandler;
        }

        public void ThrottledEventHandler(object sender, T args)
        {
            if (ShouldFireEvent())
            {
                eventHandler(sender, args);
            }
        }

        private bool ShouldFireEvent()
        {
            var now = DateTime.UtcNow;
            if (lastEvent == DateTime.MinValue || now - lastEvent > threshold)
            {
                lastEvent = now;
                return true;
            }

            return false;
        }
    }
}
