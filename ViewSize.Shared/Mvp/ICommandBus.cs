// <copyright file="ICommandBus.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;

namespace CRLFLabs.ViewSize.Mvp
{
    public interface ICommandBus
    {
        void Subscribe(string command, Action handler);

        void Publish(string command);
    }
}
