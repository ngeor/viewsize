using CRLFLabs.ViewSize.Drawing;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewSize.Tests.Drawing
{
    class RectangleFTest
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
            var r = new RectangleF(left, top, width, height);
            Assert.AreEqual(left, r.Left);
            Assert.AreEqual(top, r.Top);
            Assert.AreEqual(width, r.Width);
            Assert.AreEqual(height, r.Height);
            Assert.AreEqual(left + width, r.Right);
            Assert.AreEqual(top + height, r.Bottom);
        }

        [Test]
        public void Subtract_SameOriginAndHeight()
        {
            // arrange
            RectangleF outerRect = new RectangleF(0, 0, 100, 100);
            RectangleF innerRect = new RectangleF(0, 0, 50, 100);
            RectangleF expectedRect = new RectangleF(50, 0, 50, 100);

            // act
            RectangleF actualRect = outerRect.Subtract(innerRect);

            // assert
            Assert.AreEqual(expectedRect, actualRect);
        }

        [Test]
        public void Subtract_SameOriginAndWidth()
        {
            // arrange
            RectangleF outerRect = new RectangleF(0, 0, 100, 100);
            RectangleF innerRect = new RectangleF(0, 0, 100, 50);
            RectangleF expectedRect = new RectangleF(0, 50, 100, 50);

            // act
            RectangleF actualRect = outerRect.Subtract(innerRect);

            // assert
            Assert.AreEqual(expectedRect, actualRect);
        }

    }
}
