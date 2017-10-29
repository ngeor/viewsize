// <copyright file="IApplicationView.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;

namespace CRLFLabs.ViewSize.Mvp
{
    public interface IApplicationView : IView
    {
        event EventHandler Closing;
    }
}
