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
        private Mock<IFolderScanner> _folderScannerMock;
        private Mock<IFileUtils> _fileUtilsMock;
        private MainModel _model;

        [SetUp]
        public void SetUp()
        {
            _viewMock = new Mock<IMainView>();
            _folderScannerMock = new Mock<IFolderScanner>();
            _fileUtilsMock = new Mock<IFileUtils>();
            _presenter = new MainPresenter(
                _viewMock.Object,
                _model = new MainModel(),
                _folderScannerMock.Object,
                _fileUtilsMock.Object);

            _viewMock.Raise(v => v.Load += null, EventArgs.Empty);
        }

        [Test]
        public void OnBeginScan_NoFolderSelected_ShouldShowError()
        {
            // arrange
            _model.Folder = "";

            // act
            _viewMock.Raise(v => v.OnBeginScanClick += null, EventArgs.Empty);

            // assert
            _viewMock.Verify(v => v.ShowError("No folder selected!"));
        }

        [Test]
        public void OnBeginScan_SelectedFolderDoesNotExist_ShouldShowError()
        {
            // arrange
            _model.Folder = "test";
            _fileUtilsMock.Setup(f => f.IsDirectory("test")).Returns(false);

            // act
            _viewMock.Raise(v => v.OnBeginScanClick += null, EventArgs.Empty);

            // assert
            _viewMock.Verify(v => v.ShowError("Folder 'test' does not exist!"));
        }

        [Test]
        public void UpOneLevel_NoSelected_DoesNothing()
        {
            // arrange

            // act
            _viewMock.Raise(v => v.UpOneLevelClick += null, EventArgs.Empty);

            // assert
            Assert.IsNull(_model.Selected);
        }

        [Test]
        public void UpOneLevel_NoParent_DoesNothing()
        {
            // arrange
            var selected = new FileSystemEntry("test", null);
            _model.Selected = selected;

            // act
            _viewMock.Raise(v => v.UpOneLevelClick += null, EventArgs.Empty);

            // assert
            Assert.AreSame(selected, _model.Selected);
        }

        [Test]
        public void UpOneLevel_WithParent_SelectsParent()
        {
            // arrange
            var parent = new FileSystemEntry("test", null);
            var selected = new FileSystemEntry("child", parent);
            _model.Selected = selected;

            // act
            _viewMock.Raise(v => v.UpOneLevelClick += null, EventArgs.Empty);

            // assert
            Assert.AreSame(parent, _model.Selected);
        }
    }
}
