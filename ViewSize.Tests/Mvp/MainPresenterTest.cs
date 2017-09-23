using System;
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

        [SetUp]
        public void SetUp()
        {
            _viewMock = new Mock<IMainView>();
            _presenter = new MainPresenter(_viewMock.Object);
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
