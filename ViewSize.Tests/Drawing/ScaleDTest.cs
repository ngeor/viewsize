using CRLFLabs.ViewSize.Drawing;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewSize.Tests.Drawing
{
    public class ScaleDTest
    {
        [TestCase(1, 2)]
        [TestCase(2, 1)]
        public void CreateScale(int sx, int sy)
        {
            var scale = new ScaleD(sx, sy);
            Assert.AreEqual(sx, scale.ScaleX);
            Assert.AreEqual(sy, scale.ScaleY);
        }

        [Test]
        public void CreateScaleWithSize()
        {
            // arrange
            var sizeFrom = new SizeD(100, 200);
            var sizeTo = new SizeD(10, 10);
            var expected = new ScaleD(0.1, 0.05);

            // act
            var actual = new ScaleD(sizeFrom, sizeTo);

            // assert
            Assert.AreEqual(expected, actual);
        }

        [TestCase(1, 1, 1, 1)]
        [TestCase(2, 2, 0.5, 0.5)]
        [TestCase(0.25, 5, 4, 0.2)]
        public void Invert(double sx, double sy, double expectedX, double expectedY)
        {
            var scale = new ScaleD(sx, sy);
            var inverted = scale.Invert();
            var expected = new ScaleD(expectedX, expectedY);
            Assert.AreEqual(expected, inverted);
        }
    }
}
