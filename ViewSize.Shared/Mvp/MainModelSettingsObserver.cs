// <copyright file="MainModelSettingsObserver.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;
using CRLFLabs.ViewSize.Settings;

namespace CRLFLabs.ViewSize.Mvp
{
    public abstract class MainModelSettingsObserver : MainModelObserver
    {
        protected MainModelSettingsObserver(IMainModel model, ISettingsManager settingsManager)
            : base(model)
        {
            SettingsManager = settingsManager ?? throw new ArgumentNullException(nameof(settingsManager));
        }

        public ISettingsManager SettingsManager { get; }
    }
}
