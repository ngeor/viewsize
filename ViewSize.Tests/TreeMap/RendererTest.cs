using NUnit.Framework;
using CRLFLabs.ViewSize;
using CRLFLabs.ViewSize.TreeMap;
using System.Collections.Generic;
using Moq;
using System.Linq;
using CRLFLabs.ViewSize.Drawing;
using System.Diagnostics;
using System;
using CRLFLabs.ViewSize.IO;

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
            var renderer = new Renderer(fullBounds, folders);
            renderer.Render();

            // assert
            Assert.AreEqual(new RectangleD(0, 0, width, height), folders[0].Bounds);
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
            var renderer = new Renderer(fullBounds, folders);
            renderer.Render();

            // assert
            Assert.AreEqual(new RectangleD(0, 0, 100, 100), folders[0].Bounds);
            Assert.AreEqual(new RectangleD(0, 0, 100, 100), folders[0].Children[0].Bounds);
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
            var renderer = new Renderer(fullBounds, folders);
            renderer.Render();

            // assert
            Assert.AreEqual(new RectangleD(0, 0, 50, 100), folders[0].Bounds, "first rectangle");
            Assert.AreEqual(new RectangleD(50, 0, 50, 100), folders[1].Bounds, "second rectangle");
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
            var renderer = new Renderer(fullBounds, folders);
            renderer.Render();

            // assert
            Assert.AreEqual(new RectangleD(0, 0, 50, 50), folders[0].Bounds, "first rectangle");
            Assert.AreEqual(new RectangleD(50, 0, 50, 50), folders[1].Bounds, "second rectangle");
            Assert.AreEqual(new RectangleD(0, 50, 50, 50), folders[2].Bounds, "third rectangle");
            Assert.AreEqual(new RectangleD(50, 50, 50, 50), folders[3].Bounds, "fourth rectangle");
        }

        [Test]
        [Category("Performance")]
        public void TestPerformance()
        {
            const string path = @"C:\\Users\\ngeor\\Projects\\crlflabs";
            const int iterations = 100;

            FolderScanner fs = new FolderScanner(new FileUtils());
            var topLevelFolders = fs.Scan(path);

            Stopwatch stopwatch = Stopwatch.StartNew();
            for (var i = 0; i < iterations; i++)
            {
                var renderer = new Renderer(new RectangleD(0,0,800,600), topLevelFolders);
                renderer.Render();
            }

            stopwatch.Stop();
            Debug.WriteLine("Stopwatch: {0}", stopwatch.Elapsed);
            Assert.Less(stopwatch.Elapsed, TimeSpan.FromSeconds(1));
        }

        private static FileSystemEntry CreateFileSystemEntry(long totalSize = 1024, IReadOnlyList<FileSystemEntry> children = null)
        {
            Mock<FileSystemEntry> mock = new Mock<FileSystemEntry>("", Mock.Of<IFileSystemEntryContainer>());
            mock.SetupGet(e => e.TotalSize)
                .Returns(children != null ? children.Sum(c => c.TotalSize) : totalSize);
            mock.SetupGet(e => e.Children).Returns(children ?? Lists.Empty<FileSystemEntry>());
            return mock.Object;
        }
    }
}
