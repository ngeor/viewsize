// <copyright file="IView.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;

namespace CRLFLabs.ViewSize.Mvp
{
    public interface IView
    {
        event EventHandler Load;
    }

    public interface IView<TModel> : IView
    {
        TModel Model { get; set; }
    }
}
