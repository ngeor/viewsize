using NUnit.Framework;
using CRLFLabs.ViewSize;
using CRLFLabs.ViewSize.TreeMap;
using System.Collections.Generic;
using Moq;
using System.Linq;
using CRLFLabs.ViewSize.Drawing;
using System.Diagnostics;
using System;

namespace ViewSize.Tests.TreeMap
{
    public class RendererTest
    {
        [TestCase(100, 100)]
        [TestCase(200, 100)]
        [TestCase(100, 200)]
        public void Render_OneFile(double width, double height)
        {
            // arrange
            RectangleD fullBounds = new RectangleD(0, 0, width, height);
            var entry = CreateFileSystemEntry();
            var folders = Lists.Of(entry);

            // act
            var dataSource = Renderer.Render(fullBounds, folders);

            // assert
            Assert.AreEqual(1, dataSource.Children.Count);
            Assert.AreEqual(new RectangleD(0, 0, width, height), dataSource.Children[0].Bounds);
        }

        [Test]
        public void Render_OneFolderWithOneFile()
        {
            // arrange
            RectangleD fullBounds = new RectangleD(0, 0, 100, 100);
            var entry = CreateFileSystemEntry(
                children: Lists.Of(CreateFileSystemEntry())
            );
            var folders = Lists.Of(entry);

            // act
            var calculatedBounds = Renderer.Render(fullBounds, folders).Children;

            // assert
            Assert.AreEqual(1, calculatedBounds.Count);
            Assert.AreEqual(new RectangleD(0, 0, 100, 100), calculatedBounds[0].Bounds);
            Assert.AreEqual(new RectangleD(0, 0, 100, 100), calculatedBounds[0].Children[0].Bounds);
        }

        [Test]
        public void Render_TwoEqualFiles()
        {
            // arrange
            RectangleD fullBounds = new RectangleD(0, 0, 100, 100);

            var folders = new[]
            {
                CreateFileSystemEntry(),
                CreateFileSystemEntry()
            };

            // act
            var calculatedBounds = Renderer.Render(fullBounds, folders).Children;

            // assert
            Assert.AreEqual(2, calculatedBounds.Count);
            Assert.AreEqual(new RectangleD(0, 0, 50, 100), calculatedBounds[0].Bounds, "first rectangle");
            Assert.AreEqual(new RectangleD(50, 0, 50, 100), calculatedBounds[1].Bounds, "second rectangle");
        }

        [Test]
        public void Render_FourEqualFiles()
        {
            // arrange
            RectangleD fullBounds = new RectangleD(0, 0, 100, 100);

            var folders = new[]
            {
                CreateFileSystemEntry(),
                CreateFileSystemEntry(),
                CreateFileSystemEntry(),
                CreateFileSystemEntry()
            };

            // act
            var calculatedBounds = Renderer.Render(fullBounds, folders).Children;

            // assert
            Assert.AreEqual(4, calculatedBounds.Count);
            Assert.AreEqual(new RectangleD(0, 0, 50, 50), calculatedBounds[0].Bounds, "first rectangle");
            Assert.AreEqual(new RectangleD(50, 0, 50, 50), calculatedBounds[1].Bounds, "second rectangle");
            Assert.AreEqual(new RectangleD(0, 50, 50, 50), calculatedBounds[2].Bounds, "third rectangle");
            Assert.AreEqual(new RectangleD(50, 50, 50, 50), calculatedBounds[3].Bounds, "fourth rectangle");
        }

        [Test]
        [Category("Performance")]
        public void TestPerformance()
        {
            const string path = @"C:\\Users\\ngeor\\Projects\\crlflabs";
            const int iterations = 100;

            FolderScanner fs = new FolderScanner();
            var topLevelFolders = fs.Scan(path);

            Stopwatch stopwatch = Stopwatch.StartNew();
            for (var i = 0; i < iterations; i++)
            {
                Renderer.Render(
                    new RectangleD(0, 0, 800, 600),
                    topLevelFolders);
            }

            stopwatch.Stop();
            Debug.WriteLine("Stopwatch: {0}", stopwatch.Elapsed);
            Assert.Less(stopwatch.Elapsed, TimeSpan.FromSeconds(1));
        }

        private static FileSystemEntry CreateFileSystemEntry(long totalSize = 1024, IReadOnlyList<FileSystemEntry> children = null)
        {
            return new FileSystemEntry("", Mock.Of<IFileSystemEntryContainer>())
            {
                TotalSize = children != null ? children.Sum(c => c.TotalSize) : totalSize,
                Children = children ?? Lists.Empty<FileSystemEntry>()
            };
        }
    }
}
