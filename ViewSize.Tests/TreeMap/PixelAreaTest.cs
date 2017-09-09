using CRLFLabs.ViewSize.TreeMap;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewSize.Tests.TreeMap
{
    public class PixelAreaTest
    {
        [Test]
        public void FillOneDimension_Half_Horizontally()
        {
            PixelArea pixelArea = new PixelArea(50);
            var result = pixelArea.FillOneDimension(new RectangleF(0, 0, 10, 10), false);
            Assert.AreEqual(new RectangleF(0, 0, 10, 5), result);
        }

        [Test]
        public void FillOneDimension_Half_Vertically()
        {
            PixelArea pixelArea = new PixelArea(50);
            var result = pixelArea.FillOneDimension(new RectangleF(0, 0, 10, 10), true);
            Assert.AreEqual(new RectangleF(0, 0, 5, 10), result);
        }

    }
}
