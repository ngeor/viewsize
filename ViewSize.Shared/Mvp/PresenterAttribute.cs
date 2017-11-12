// <copyright file="PresenterAttribute.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;

namespace CRLFLabs.ViewSize.Mvp
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class PresenterAttribute : Attribute
    {
        public PresenterAttribute(Type presenterType)
        {
            PresenterType = presenterType;
        }

        public Type PresenterType { get; }
    }
}
