// <copyright file="TypeMapping.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;

namespace CRLFLabs.ViewSize.IoC
{
    public class TypeMapping
    {
        public TypeMapping(Type actualType, bool singleton)
        {
            this.ActualType = actualType;
            this.Singleton = singleton;
        }

        public Type ActualType { get; }

        public bool Singleton { get; }
    }
}
