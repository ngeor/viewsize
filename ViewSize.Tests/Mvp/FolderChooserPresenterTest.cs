using System;
using CRLFLabs.ViewSize.Mvp;
using CRLFLabs.ViewSize.Settings;
using Moq;
using NUnit.Framework;

namespace ViewSize.Tests.Mvp
{
    [TestFixture]
    public class FolderChooserPresenterTest
    {
        private FolderChooserPresenter _presenter;
        private Mock<IFolderChooserView> _viewMock;
        private Mock<ISettingsManager> _settingsManagerMock;
        private Settings _settings;
        private CommandBus _commandBus;

        [SetUp]
        public void SetUp()
        {
            _viewMock = new Mock<IFolderChooserView>();
            _settings = new Settings
            {
                SelectedFolder = "settings value"
            };
            _settingsManagerMock = new Mock<ISettingsManager>();
            _settingsManagerMock.SetupGet(m => m.Settings).Returns(_settings);
            _viewMock.SetupProperty(v => v.Model);
            _commandBus = new CommandBus();
            _presenter = new FolderChooserPresenter(
                _viewMock.Object,
                Mock.Of<IMainModel>(),
                _settingsManagerMock.Object,
                _commandBus
            );

            _viewMock.Raise(v => v.Load += null, EventArgs.Empty);
        }

        [Test]
        public void OnSelectFolder_WhenViewReturnsFolder_ShouldUpdateModel()
        {
            // arrange
            _viewMock.Setup(v => v.SelectFolder()).Returns("some path");

            // act
            _viewMock.Raise(v => v.OnSelectFolderClick += null, EventArgs.Empty);

            // assert
            Assert.AreEqual("some path", _viewMock.Object.Model.Folder);
        }

        [Test]
        public void OnSelectFolder_WhenViewReturnsNull_ShouldNotUpdateModel()
        {
            // arrange
            _viewMock.Setup(v => v.SelectFolder()).Returns((string)null);

            // act
            _viewMock.Raise(v => v.OnSelectFolderClick += null, EventArgs.Empty);

            // assert
            Assert.AreEqual("settings value", _viewMock.Object.Model.Folder);
        }

        [Test]
        public void PublishCommand_UpdatesModel()
        {
            // arrange
            _viewMock.Setup(v => v.SelectFolder()).Returns("some path");

            // act
            _commandBus.Publish("SelectFolder");

            // assert
            Assert.AreEqual("some path", _viewMock.Object.Model.Folder);
        }
    }
}
