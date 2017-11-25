// <copyright file="RectangleDTest.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using CRLFLabs.ViewSize.Drawing;
using NUnit.Framework;

namespace ViewSize.Tests.Drawing
{
    public class RectangleDTest
    {
        public class Ctor
        {
            [TestCase(0, 0, 100, 100)]
            [TestCase(0, 0, 200, 100)]
            [TestCase(0, 0, 100, 200)]
            [TestCase(50, 20, 200, 100)]
            [TestCase(50, 20, 100, 200)]
            [TestCase(20, 30, 200, 100)]
            [TestCase(20, 40, 100, 200)]
            public void CreateRectangle(double left, double top, double width, double height)
            {
                var r = new RectangleD(left, top, width, height);
                Assert.AreEqual(left, r.Left);
                Assert.AreEqual(top, r.Top);
                Assert.AreEqual(width, r.Width);
                Assert.AreEqual(height, r.Height);
                Assert.AreEqual(left + width, r.Right);
                Assert.AreEqual(top + height, r.Bottom);
            }
        }

        public class Subtract
        {
            [Test]
            public void SameOriginAndHeight()
            {
                // arrange
                RectangleD outerRect = new RectangleD(0, 0, 100, 100);
                RectangleD innerRect = new RectangleD(0, 0, 50, 100);
                RectangleD expectedRect = new RectangleD(50, 0, 50, 100);

                // act
                RectangleD actualRect = outerRect.Subtract(innerRect);

                // assert
                Assert.AreEqual(expectedRect, actualRect);
            }

            [Test]
            public void SameOriginAndWidth()
            {
                // arrange
                RectangleD outerRect = new RectangleD(0, 0, 100, 100);
                RectangleD innerRect = new RectangleD(0, 0, 100, 50);
                RectangleD expectedRect = new RectangleD(0, 50, 100, 50);

                // act
                RectangleD actualRect = outerRect.Subtract(innerRect);

                // assert
                Assert.AreEqual(expectedRect, actualRect);
            }
        }

        public class Contains
        {
            [TestCase(0, 0, 100, 100, 50, 50, true)]
            [TestCase(0, 0, 100, 100, 100, 50, false)]
            [TestCase(0, 0, 100, 100, 50, 100, false)]
            [TestCase(10, 10, 10, 10, 50, 50, false)]
            [TestCase(10, 10, 10, 10, 5, 5, false)]
            public void ContainsPoint(int left, int top, int width, int height, int x, int y, bool contains)
            {
                RectangleD rect = new RectangleD(left, top, width, height);
                PointD point = new PointD(x, y);
                Assert.AreEqual(contains, rect.Contains(point));
            }
        }
    }
}
