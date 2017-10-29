// <copyright file="FolderChooserPresenterTest.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

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
        private FolderChooserPresenter presenter;
        private Mock<IFolderChooserView> viewMock;
        private Mock<ISettingsManager> settingsManagerMock;
        private Settings settings;
        private CommandBus commandBus;

        [SetUp]
        public void SetUp()
        {
            this.viewMock = new Mock<IFolderChooserView>();
            this.settings = new Settings
            {
                SelectedFolder = "settings value"
            };
            this.settingsManagerMock = new Mock<ISettingsManager>();
            this.settingsManagerMock.SetupGet(m => m.Settings).Returns(this.settings);
            this.viewMock.SetupProperty(v => v.Model);
            this.commandBus = new CommandBus();
            this.presenter = new FolderChooserPresenter(
                this.viewMock.Object,
                Mock.Of<IMainModel>(),
                this.settingsManagerMock.Object,
                this.commandBus);

            this.viewMock.Raise(v => v.Load += null, EventArgs.Empty);
        }

        [Test]
        public void OnSelectFolder_WhenViewReturnsFolder_ShouldUpdateModel()
        {
            // arrange
            this.viewMock.Setup(v => v.SelectFolder()).Returns("some path");

            // act
            this.viewMock.Raise(v => v.OnSelectFolderClick += null, EventArgs.Empty);

            // assert
            Assert.AreEqual("some path", this.viewMock.Object.Model.Folder);
        }

        [Test]
        public void OnSelectFolder_WhenViewReturnsNull_ShouldNotUpdateModel()
        {
            // arrange
            this.viewMock.Setup(v => v.SelectFolder()).Returns((string)null);

            // act
            this.viewMock.Raise(v => v.OnSelectFolderClick += null, EventArgs.Empty);

            // assert
            Assert.AreEqual("settings value", this.viewMock.Object.Model.Folder);
        }

        [Test]
        public void PublishCommand_UpdatesModel()
        {
            // arrange
            this.viewMock.Setup(v => v.SelectFolder()).Returns("some path");

            // act
            this.commandBus.Publish("SelectFolder");

            // assert
            Assert.AreEqual("some path", this.viewMock.Object.Model.Folder);
        }
    }
}
