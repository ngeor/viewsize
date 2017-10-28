using System;
using CRLFLabs.ViewSize.Mvp;
using Moq;
using NUnit.Framework;

namespace ViewSize.Tests.Mvp
{
    public class MenuPresenterTest
    {
        private MenuPresenter _menuPresenter;
        private Mock<IMenuView> _viewMock;
        private MainModel _mainModel;
        private Mock<ICommandBus> _commandBusMock;

        [SetUp]
        public void SetUp()
        {
            _viewMock = new Mock<IMenuView>();
            _mainModel = new MainModel();
            _commandBusMock = new Mock<ICommandBus>();
            _menuPresenter = new MenuPresenter(
                _viewMock.Object,
                _mainModel,
                _commandBusMock.Object
            );

            _viewMock.Raise(v => v.Load += null, EventArgs.Empty);
        }

        [Test]
        public void FileOpenClick_ShowsMainWindow()
        {
            _viewMock.Raise(v => v.FileOpenClick += null, EventArgs.Empty);
            _viewMock.Verify(v => v.ShowMainWindow());
        }

        [Test]
        public void FileOpenClick_PublishesCommand()
        {
            _viewMock.Raise(v => v.FileOpenClick += null, EventArgs.Empty);
            _commandBusMock.Verify(v => v.Publish("SelectFolder"));
        }
    }
}
