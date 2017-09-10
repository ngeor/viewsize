using CRLFLabs.ViewSize.Drawing;
using NUnit.Framework;

namespace ViewSize.Tests.Drawing
{
    public class SizeDTest
    {
        [TestCase(100, 100)]
        [TestCase(100, 200)]
        [TestCase(200, 100)]
        public void CreateSize(double width, double height)
        {
            var s = new SizeD(width, height);
            Assert.AreEqual(width, s.Width);
            Assert.AreEqual(height, s.Height);
        }
    }
}
