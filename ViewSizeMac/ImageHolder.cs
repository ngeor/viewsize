// <copyright file="ImageHolder.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;
using AppKit;

namespace ViewSizeMac
{
    class ImageHolder : IDisposable
    {
        private NSImage _lastValue;

        public bool HasValue => Image != null;

        public NSImage Image
        {
            get
            {
                return _lastValue;
            }

            set
            {
                Dispose();
                _lastValue = value;
            }
        }

        public void Dispose()
        {
            if (_lastValue != null)
            {
                _lastValue.Dispose();
                _lastValue = null;
            }
        }
    }
}
