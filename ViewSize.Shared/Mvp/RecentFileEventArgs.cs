// <copyright file="RecentFileEventArgs.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;

namespace CRLFLabs.ViewSize.Mvp
{
    public class RecentFileEventArgs : EventArgs
    {
        public RecentFileEventArgs(string filename)
        {
            Filename = filename;
        }

        public string Filename { get; }
    }
}
