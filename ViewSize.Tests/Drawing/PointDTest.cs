// <copyright file="PointDTest.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using CRLFLabs.ViewSize.Drawing;
using NUnit.Framework;

namespace ViewSize.Tests.Drawing
{
    public class PointDTest
    {
        [TestCase(1, 2)]
        [TestCase(2, 1)]
        public void CreatePoint(int x, int y)
        {
            var point = new PointD(x, y);
            Assert.AreEqual(x, point.X);
            Assert.AreEqual(y, point.Y);
        }
    }
}
