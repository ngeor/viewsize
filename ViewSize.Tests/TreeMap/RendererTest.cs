using NUnit.Framework;
using CRLFLabs.ViewSize;
using CRLFLabs.ViewSize.TreeMap;
using System.Collections.Generic;
using Moq;

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

        [Test]
        public void Render_OneFolder()
        {
            // arrange
            RectangleF fullBounds = new RectangleF(0, 0, 100, 100);
            var mockFileSystemEntry = CreateFileSystemEntryMock();
            var folders = Lists.Of(mockFileSystemEntry.Object);

            List<RectangleF> calculatedBounds = new List<RectangleF>();

            // accumulate all render results
            renderer.DoRender = (bounds) => calculatedBounds.Add(bounds);

            // act
            renderer.Render(fullBounds, folders);

            // assert
            Assert.AreEqual(1, calculatedBounds.Count);
            Assert.AreEqual(new RectangleF(0, 0, 100, 100), calculatedBounds[0]);
        }

        [Test]
        public void Render_TwoEqualFolders()
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
        public void Render_FourEqualFolders()
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

        private static Mock<IFileSystemEntry> CreateFileSystemEntryMock(long totalSize = 1024)
        {
            return FluentMoq.Create<IFileSystemEntry>(
                x =>
                    {
                        x.SetupGet(f => f.Children).Returns(Lists.Empty<IFileSystemEntry>());
                        x.SetupGet(f => f.TotalSize).Returns(totalSize);
                    }
            );
        }
    }
}
