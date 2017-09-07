using CRLFLabs.ViewSize.TreeMap;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewSize.Tests
{
    public class SizeFTest
    {
        [Test]
        public void Create()
        {
            var s = new SizeF(100, 200);
            Assert.AreEqual(100, s.Width);
            Assert.AreEqual(200, s.Height);
        }
    }
}
