// <copyright file="CanExecuteEventArgs.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;

namespace CRLFLabs.ViewSize.Mvp
{
    public class CanExecuteEventArgs : EventArgs
    {
        public bool CanExecute { get; set; }
    }
}
