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
        private Mock<IFolderChooserModel> _model;
        private Mock<IFolderChooserView> _view;
        private Mock<ISettingsManager> _settingsManager;
        private Settings _settings;

        [SetUp]
        public void SetUp()
        {
            _view = new Mock<IFolderChooserView>();
            _model = new Mock<IFolderChooserModel>();
            _settings = new Settings
            {
                SelectedFolder = "settings value"
            };
            _settingsManager = new Mock<ISettingsManager>();
            _settingsManager.SetupGet(m => m.Settings).Returns(_settings);
            _model.SetupProperty(m => m.Folder);
            _model.Object.Folder = "value before settings";
            _presenter = new FolderChooserPresenter(
                _view.Object,
                _model.Object,
                _settingsManager.Object
            );    
        }

        [Test]
        public void OnSelectFolder_WhenViewReturnsFolder_ShouldUpdateModel()
        {
            // arrange
            _view.Setup(v => v.SelectFolder()).Returns("some path");

            // act
            _presenter.OnSelectFolder();

            // assert
            Assert.AreEqual("some path", _model.Object.Folder);
        }

        [Test]
        public void OnSelectFolder_WhenViewReturnsNull_ShouldNotUpdateModel()
        {
            // arrange
            _view.Setup(v => v.SelectFolder()).Returns((string)null);

            // act
            _presenter.OnSelectFolder();

            // assert
            Assert.AreEqual("settings value", _model.Object.Folder);
        }
    }
}
