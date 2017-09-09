using NUnit.Framework;
using CRLFLabs.ViewSize;
using CRLFLabs.ViewSize.TreeMap;
using System.Collections.Generic;
using Moq;
using System.Linq;

namespace ViewSize.Tests.TreeMap
{
    public class RendererTest
    {
        private Renderer renderer;

        [SetUp]
        public void SetUp()
        {
            renderer = new Renderer();
        }

        [TestCase(100, 100)]
        [TestCase(200, 100)]
        [TestCase(100, 200)]
        public void Render_OneFile(double width, double height)
        {
            // arrange
            RectangleF fullBounds = new RectangleF(0, 0, width, height);
            var mockFileSystemEntry = CreateFileSystemEntryMock();
            var folders = Lists.Of(mockFileSystemEntry.Object);

            List<RectangleF> calculatedBounds = new List<RectangleF>();

            // accumulate all render results
            renderer.DoRender = (bounds) => calculatedBounds.Add(bounds);

            // act
            renderer.Render(fullBounds, folders);

            // assert
            Assert.AreEqual(1, calculatedBounds.Count);
            Assert.AreEqual(new RectangleF(0, 0, width, height), calculatedBounds[0]);
        }

        [Test]
        public void Render_OneFolderWithOneFile()
        {
            // arrange
            RectangleF fullBounds = new RectangleF(0, 0, 100, 100);
            var mockFileSystemEntry = CreateFileSystemEntryMock(
                children: Lists.Of(CreateFileSystemEntryMock()).ToListOfInstances()
            );
            var folders = Lists.Of(mockFileSystemEntry.Object);

            List<RectangleF> calculatedBounds = new List<RectangleF>();

            // accumulate all render results
            renderer.DoRender = (bounds) => calculatedBounds.Add(bounds);

            // act
            renderer.Render(fullBounds, folders);

            // assert
            Assert.AreEqual(2, calculatedBounds.Count);
            Assert.AreEqual(new RectangleF(0, 0, 100, 100), calculatedBounds[0]);
            Assert.AreEqual(new RectangleF(0, 0, 100, 100), calculatedBounds[1]);
        }

        [Test]
        public void Render_TwoEqualFiles()
        {
            // arrange
            RectangleF fullBounds = new RectangleF(0, 0, 100, 100);

            var mockFileSystemEntries = new[]
            {
                CreateFileSystemEntryMock(),
                CreateFileSystemEntryMock()
            };

            var folders = mockFileSystemEntries.ToListOfInstances();

            List<RectangleF> calculatedBounds = new List<RectangleF>();

            // accumulate all render results
            renderer.DoRender = (bounds) => calculatedBounds.Add(bounds);

            // act
            renderer.Render(fullBounds, folders);

            // assert
            Assert.AreEqual(2, calculatedBounds.Count);
            Assert.AreEqual(new RectangleF(0, 0, 50, 100), calculatedBounds[0], "first rectangle");
            Assert.AreEqual(new RectangleF(50, 0, 50, 100), calculatedBounds[1], "second rectangle");
        }

        [Test]
        public void Render_FourEqualFiles()
        {
            // arrange
            RectangleF fullBounds = new RectangleF(0, 0, 100, 100);

            var mockFileSystemEntries = new[]
            {
                CreateFileSystemEntryMock(),
                CreateFileSystemEntryMock(),
                CreateFileSystemEntryMock(),
                CreateFileSystemEntryMock()
            };

            var folders = mockFileSystemEntries.ToListOfInstances();

            List<RectangleF> calculatedBounds = new List<RectangleF>();

            // accumulate all render results
            renderer.DoRender = (bounds) => calculatedBounds.Add(bounds);

            // act
            renderer.Render(fullBounds, folders);

            // assert
            Assert.AreEqual(4, calculatedBounds.Count);
            Assert.AreEqual(new RectangleF(0, 0, 50, 50), calculatedBounds[0], "first rectangle");
            Assert.AreEqual(new RectangleF(50, 0, 50, 50), calculatedBounds[1], "second rectangle");
            Assert.AreEqual(new RectangleF(0, 50, 50, 50), calculatedBounds[2], "third rectangle");
            Assert.AreEqual(new RectangleF(50, 50, 50, 50), calculatedBounds[3], "fourth rectangle");
        }

        private static Mock<IFileSystemEntry> CreateFileSystemEntryMock(long totalSize = 1024, IList<IFileSystemEntry> children = null)
        {
            return FluentMoq.Create<IFileSystemEntry>(
                x =>
                    {
                        x.SetupGet(f => f.Children)
                            .Returns(children ?? Lists.Empty<IFileSystemEntry>());
                        x.SetupGet(f => f.TotalSize)
                            .Returns(children != null ? children.Sum(c => c.TotalSize) : totalSize);
                    }
            );
        }
    }
}
