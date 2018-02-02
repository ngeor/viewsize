// <copyright file="OwnsSimple.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;
using CRLFLabs.ViewSize.IoC;
using NUnit.Framework;

namespace ViewSize.Tests.IoC
{

    public class OwnsSimple
    {
        public OwnsSimple(Simple dependency)
        {
            Dependency = dependency;
        }

        public Simple Dependency { get; }
    }
}
