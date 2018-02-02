// <copyright file="MainModelObserver.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;

namespace CRLFLabs.ViewSize.Mvp
{
    public abstract class MainModelObserver
    {
        protected MainModelObserver(IMainModel model)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
        }

        public IMainModel Model { get; }
    }
}
