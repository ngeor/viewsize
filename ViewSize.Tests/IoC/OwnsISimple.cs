// <copyright file="OwnsISimple.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;
using CRLFLabs.ViewSize.IoC;
using NUnit.Framework;

namespace ViewSize.Tests.IoC
{

    public class OwnsISimple
    {
        public OwnsISimple(ISimple dependency)
        {
            Dependency = dependency;
        }

        public ISimple Dependency { get; }
    }
}
