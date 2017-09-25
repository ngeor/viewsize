using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CRLFLabs.ViewSize.IO;
using Moq;
using NUnit.Framework;

namespace ViewSize.Tests.IO
{
    [TestFixture]
    public class FolderScannerTest
    {
        private FolderScanner _folderScanner;
        private Mock<IFileUtils> _fileUtilsMock;

        [SetUp]
        public void SetUp()
        {
            _fileUtilsMock = new Mock<IFileUtils>();
            _folderScanner = new FolderScanner(_fileUtilsMock.Object);
        }

        [Test]
        public void Cancel_ShouldCancelScan()
        {
            // arrange
            const int count = 100000;
            _fileUtilsMock.Setup(p => p.EnumerateFileSystemEntries("test"))
                          .Returns(Enumerable.Repeat("nope", count));

            Task.Run(async () => {
                await Task.Delay(500); // so that it start scanning

                // act
                _folderScanner.Cancel();
            });

            var result = _folderScanner.Scan("test");

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.Less(result[0].Children.Count, count);
            Assert.IsFalse(_folderScanner.CancelRequested);
        }
    }
}
