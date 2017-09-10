using CRLFLabs.ViewSize.Drawing;
using CRLFLabs.ViewSize.TreeMap;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewSize.Tests.Drawing
{
    public class SizeFTest
    {
        [TestCase(100, 100)]
        [TestCase(100, 200)]
        [TestCase(200, 100)]
        public void CreateSize(double width, double height)
        {
            var s = new SizeF(width, height);
            Assert.AreEqual(width, s.Width);
            Assert.AreEqual(height, s.Height);
        }
    }
}
