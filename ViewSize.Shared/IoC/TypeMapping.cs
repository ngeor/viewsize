// <copyright file="TypeMapping.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;

namespace CRLFLabs.ViewSize.IoC
{
    public class TypeMapping
    {
        public TypeMapping(Type from, Type to, bool singleton)
        {
            From = from;
            To = to;
            Singleton = singleton;
        }

        public Type From { get; }

        public Type To { get; }

        public bool Singleton { get; }
    }
}
