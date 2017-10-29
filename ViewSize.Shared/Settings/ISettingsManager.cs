// <copyright file="ISettingsManager.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

namespace CRLFLabs.ViewSize.Settings
{
    public interface ISettingsManager
    {
        Settings Settings { get; }

        void Save();
    }
}
