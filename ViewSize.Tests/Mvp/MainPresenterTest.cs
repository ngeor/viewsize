using System;
using CRLFLabs.ViewSize;
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

        [SetUp]
        public void SetUp()
        {
            _viewMock = new Mock<IMainView>();
            _folderScannerMock = new Mock<IFolderScanner>();
            _presenter = new MainPresenter(
                _viewMock.Object,
                _folderScannerMock.Object);
        }

        [Test]
        public void OnBeginScan_NoFolderSelected_ShouldShowError()
        {
            // arrange
            _viewMock.SetupGet(v => v.SelectedFolder).Returns("");

            // act
            _presenter.OnBeginScan();

            // assert
            _viewMock.Verify(v => v.ShowError("No folder selected!"));
        }
    }
}
