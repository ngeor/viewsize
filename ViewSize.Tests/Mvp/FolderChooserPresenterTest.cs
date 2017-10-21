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
        private Mock<IFolderChooserView> _view;
        private Mock<ISettingsManager> _settingsManager;
        private Settings _settings;

        [SetUp]
        public void SetUp()
        {
            _view = new Mock<IFolderChooserView>();
            _settings = new Settings
            {
                SelectedFolder = "settings value"
            };
            _settingsManager = new Mock<ISettingsManager>();
            _settingsManager.SetupGet(m => m.Settings).Returns(_settings);
            _view.SetupProperty(v => v.Model);
            _presenter = new FolderChooserPresenter(
                _view.Object,
                _settingsManager.Object
            );

            _view.Raise(v => v.Load += null, EventArgs.Empty);
        }

        [Test]
        public void OnSelectFolder_WhenViewReturnsFolder_ShouldUpdateModel()
        {
            // arrange
            _view.Setup(v => v.SelectFolder()).Returns("some path");

            // act
            _view.Raise(v => v.OnSelectFolderClick += null, EventArgs.Empty);

            // assert
            Assert.AreEqual("some path", _view.Object.Model.Folder);
        }

        [Test]
        public void OnSelectFolder_WhenViewReturnsNull_ShouldNotUpdateModel()
        {
            // arrange
            _view.Setup(v => v.SelectFolder()).Returns((string)null);

            // act
            _view.Raise(v => v.OnSelectFolderClick += null, EventArgs.Empty);

            // assert
            Assert.AreEqual("settings value", _view.Object.Model.Folder);
        }
    }
}
