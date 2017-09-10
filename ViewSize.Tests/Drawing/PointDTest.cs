using CRLFLabs.ViewSize.Drawing;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewSize.Tests.Drawing
{
    public class PointDTest
    {
        [TestCase(1,2)]
        [TestCase(2,1)]
        public void CreatePoint(int x, int y)
        {
            var point = new PointD(x, y);
            Assert.AreEqual(x, point.X);
            Assert.AreEqual(y, point.Y);
        }

        [Test]
        public void Scale()
        {
            // arrange
            var point = new PointD(2, 4);
            var scale = new ScaleD(2, 3);
            var expected = new PointD(4, 12);

            // act
            var actual = point.Scale(scale);

            // assert
            Assert.AreEqual(expected, actual);
        }
    }
}
