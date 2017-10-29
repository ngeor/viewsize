// <copyright file="FluentMoq.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using Moq;

namespace ViewSize.Tests
{
    // TODO Move to NGSoftware.Moq?
    public static class FluentMoq
    {
        public static Mock<T> Create<T>(Action<Mock<T>> setup)
            where T : class
        {
            var result = new Mock<T>();
            setup(result);
            return result;
        }

        public static IList<T> ToListOfInstances<T>(this IEnumerable<Mock<T>> mocks)
            where T : class
        {
            return mocks.Select(m => m.Object).ToList();
        }
    }
}
