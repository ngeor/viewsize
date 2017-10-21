using System;
using CRLFLabs.ViewSize;
using CRLFLabs.ViewSize.IO;
using CRLFLabs.ViewSize.Mvp;
using Moq;
using NUnit.Framework;

namespace ViewSize.Tests.Mvp
{
    [TestFixture]
    public class MainPresenterTest
    {
        private MainPresenter _presenter;
        private Mock<IMainView> _viewMock;
        private Mock<ITreeMapView> _treeMapViewMock;
        private Mock<IFolderScanner> _folderScannerMock;
        private Mock<IFileUtils> _fileUtilsMock;

        [SetUp]
        public void SetUp()
        {
            _viewMock = new Mock<IMainView>();
            _treeMapViewMock = new Mock<ITreeMapView>();
            _viewMock.SetupGet(v => v.TreeMapView).Returns(_treeMapViewMock.Object);
            _folderScannerMock = new Mock<IFolderScanner>();
            _fileUtilsMock = new Mock<IFileUtils>();
            _presenter = new MainPresenter(
                _viewMock.Object,
                _folderScannerMock.Object,
                _fileUtilsMock.Object);

            _viewMock.Raise(v => v.Load += null, EventArgs.Empty);
        }

        [Test]
        public void OnBeginScan_NoFolderSelected_ShouldShowError()
        {
            // arrange
            _viewMock.SetupGet(v => v.SelectedFolder).Returns("");

            // act
            _viewMock.Raise(v => v.OnBeginScanClick += null, EventArgs.Empty);

            // assert
            _viewMock.Verify(v => v.ShowError("No folder selected!"));
        }

        [Test]
        public void OnBeginScan_SelectedFolderDoesNotExist_ShouldShowError()
        {
            // arrange
            _viewMock.SetupGet(v => v.SelectedFolder).Returns("test");
            _fileUtilsMock.Setup(f => f.IsDirectory("test")).Returns(false);

            // act
            _viewMock.Raise(v => v.OnBeginScanClick += null, EventArgs.Empty);

            // assert
            _viewMock.Verify(v => v.ShowError("Folder 'test' does not exist!"));
        }
    }
}
