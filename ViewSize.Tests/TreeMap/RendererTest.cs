using NUnit.Framework;
using CRLFLabs.ViewSize;
using CRLFLabs.ViewSize.TreeMap;
using System.Collections.Generic;
using Moq;
using System.Linq;
using CRLFLabs.ViewSize.Drawing;

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
            RectangleD fullBounds = new RectangleD(0, 0, width, height);
            var mockFileSystemEntry = CreateFileSystemEntryMock();
            var folders = Lists.Of(mockFileSystemEntry.Object);

            // act
            var dataSource = renderer.Render(fullBounds, folders);

            // assert
            Assert.AreEqual(1, dataSource.FoldersWithDrawSize.Count);
            Assert.AreEqual(new RectangleD(0, 0, width, height), dataSource.FoldersWithDrawSize[0].DrawSize);
        }

        [Test]
        public void Render_OneFolderWithOneFile()
        {
            // arrange
            RectangleD fullBounds = new RectangleD(0, 0, 100, 100);
            var mockFileSystemEntry = CreateFileSystemEntryMock(
                children: Lists.Of(CreateFileSystemEntryMock()).ToListOfInstances()
            );
            var folders = Lists.Of(mockFileSystemEntry.Object);

            // act
            var calculatedBounds = renderer.Render(fullBounds, folders).FoldersWithDrawSize;

            // assert
            Assert.AreEqual(1, calculatedBounds.Count);
            Assert.AreEqual(new RectangleD(0, 0, 100, 100), calculatedBounds[0].DrawSize);
            Assert.AreEqual(new RectangleD(0, 0, 100, 100), calculatedBounds[0].Children[0].DrawSize);
        }

        [Test]
        public void Render_TwoEqualFiles()
        {
            // arrange
            RectangleD fullBounds = new RectangleD(0, 0, 100, 100);

            var mockFileSystemEntries = new[]
            {
                CreateFileSystemEntryMock(),
                CreateFileSystemEntryMock()
            };

            var folders = mockFileSystemEntries.ToListOfInstances();

            // act
            var calculatedBounds = renderer.Render(fullBounds, folders).FoldersWithDrawSize;

            // assert
            Assert.AreEqual(2, calculatedBounds.Count);
            Assert.AreEqual(new RectangleD(0, 0, 50, 100), calculatedBounds[0].DrawSize, "first rectangle");
            Assert.AreEqual(new RectangleD(50, 0, 50, 100), calculatedBounds[1].DrawSize, "second rectangle");
        }

        [Test]
        public void Render_FourEqualFiles()
        {
            // arrange
            RectangleD fullBounds = new RectangleD(0, 0, 100, 100);

            var mockFileSystemEntries = new[]
            {
                CreateFileSystemEntryMock(),
                CreateFileSystemEntryMock(),
                CreateFileSystemEntryMock(),
                CreateFileSystemEntryMock()
            };

            var folders = mockFileSystemEntries.ToListOfInstances();

            // act
            var calculatedBounds = renderer.Render(fullBounds, folders).FoldersWithDrawSize;

            // assert
            Assert.AreEqual(4, calculatedBounds.Count);
            Assert.AreEqual(new RectangleD(0, 0, 50, 50), calculatedBounds[0].DrawSize, "first rectangle");
            Assert.AreEqual(new RectangleD(50, 0, 50, 50), calculatedBounds[1].DrawSize, "second rectangle");
            Assert.AreEqual(new RectangleD(0, 50, 50, 50), calculatedBounds[2].DrawSize, "third rectangle");
            Assert.AreEqual(new RectangleD(50, 50, 50, 50), calculatedBounds[3].DrawSize, "fourth rectangle");
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
