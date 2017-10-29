// <copyright file="FolderScannerTest.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System.Linq;
using System.Threading.Tasks;
using CRLFLabs.ViewSize.IO;
using Moq;
using NUnit.Framework;

namespace ViewSize.Tests.IO
{
    [TestFixture]
    public class FolderScannerTest
    {
        private FolderScanner folderScanner;
        private Mock<IFileUtils> fileUtilsMock;

        [SetUp]
        public void SetUp()
        {
            this.fileUtilsMock = new Mock<IFileUtils>();
            this.folderScanner = new FolderScanner(this.fileUtilsMock.Object);
        }

        [Test]
        public void Cancel_ShouldCancelScan()
        {
            // arrange
            const int count = 1000000;
            this.fileUtilsMock.Setup(p => p.EnumerateFileSystemEntries("test"))
                          .Returns(Enumerable.Repeat("nope", count));

            Task.Run(async () =>
            {
                await Task.Delay(500); // so that it start scanning

                // act
                this.folderScanner.Cancel();
            });

            var result = this.folderScanner.Scan("test");

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.Less(result[0].Children.Count, count);
            Assert.IsFalse(this.folderScanner.CancelRequested);
        }
    }
}
