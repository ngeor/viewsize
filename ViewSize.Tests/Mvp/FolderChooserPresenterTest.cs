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
            viewMock = new Mock<IFolderChooserView>();
            settings = new Settings
            {
                SelectedFolder = "/tmp",
                RecentFolders = new[]
                {
                    "/tmp"
                }
            };
            settingsManagerMock = new Mock<ISettingsManager>();
            settingsManagerMock.SetupGet(m => m.Settings).Returns(settings);
            viewMock.SetupProperty(v => v.Model);
            commandBus = new CommandBus();
            presenter = new FolderChooserPresenter(
                viewMock.Object,
                Mock.Of<IMainModel>(),
                settingsManagerMock.Object,
                commandBus);

            viewMock.Raise(v => v.Load += null, EventArgs.Empty);
        }

        [Test]
        public void OnSelectFolder_WhenViewReturnsFolder_ShouldUpdateModel()
        {
            // arrange
            viewMock.Setup(v => v.SelectFolder()).Returns("some path");

            // act
            viewMock.Raise(v => v.OnSelectFolderClick += null, EventArgs.Empty);

            // assert
            Assert.AreEqual("some path", viewMock.Object.Model.Folder);
        }

        [Test]
        public void OnSelectFolder_WhenViewReturnsNull_ShouldNotUpdateModel()
        {
            // arrange
            viewMock.Setup(v => v.SelectFolder()).Returns((string)null);

            // act
            viewMock.Raise(v => v.OnSelectFolderClick += null, EventArgs.Empty);

            // assert
            Assert.AreEqual("/tmp", viewMock.Object.Model.Folder);
        }

        [Test]
        public void OnSelectFolder_WhenViewReturnsFolder_ShouldUpdateSettings()
        {
            // arrange
            viewMock.Setup(v => v.SelectFolder()).Returns("some path");

            // act
            viewMock.Raise(v => v.OnSelectFolderClick += null, EventArgs.Empty);

            // assert
            Assert.AreEqual("some path", settings.SelectedFolder);
        }

        [Test]
        public void OnSelectFolder_WhenViewReturnsFolder_ShouldUpdateSettingsRecentFolders()
        {
            // arrange
            viewMock.Setup(v => v.SelectFolder()).Returns("some path");

            // act
            viewMock.Raise(v => v.OnSelectFolderClick += null, EventArgs.Empty);

            // assert
            var expected = new[]
            {
                "some path",
                "/tmp"
            };

            CollectionAssert.AreEqual(expected, settings.RecentFolders);
        }

        [Test]
        public void OnSelectFolder_WhenViewReturnsFolder_ShouldNotifyViewOfRecentFolder()
        {
            // arrange
            viewMock.Setup(v => v.SelectFolder()).Returns("some path");

            // act
            viewMock.Raise(v => v.OnSelectFolderClick += null, EventArgs.Empty);

            // assert
            viewMock.Verify(v => v.AddRecentFolder("some path"));
        }

        [Test]
        public void OnSelectFolder_WhenViewSupportsRecentFolders_ShouldNotUpdateSettingsRecentFolders()
        {
            // arrange
            viewMock.Setup(v => v.SelectFolder()).Returns("some path");
            viewMock.SetupGet(v => v.SupportsRecentFolders).Returns(true);

            // act
            viewMock.Raise(v => v.OnSelectFolderClick += null, EventArgs.Empty);

            // assert
            var expected = new[]
            {
                "/tmp"
            };

            CollectionAssert.AreEqual(expected, settings.RecentFolders);
        }

        [Test]
        public void OnSelectFolder_WhenViewSupportsRecentFolders_ShouldNotNotifyViewOfRecentFolder()
        {
            // arrange
            viewMock.Setup(v => v.SelectFolder()).Returns("some path");
            viewMock.SetupGet(v => v.SupportsRecentFolders).Returns(true);

            // act
            viewMock.Raise(v => v.OnSelectFolderClick += null, EventArgs.Empty);

            // assert
            viewMock.Verify(v => v.AddRecentFolder("some path"), Times.Never);
        }

        [Test]
        public void PublishCommand_UpdatesModel()
        {
            // arrange
            viewMock.Setup(v => v.SelectFolder()).Returns("some path");

            // act
            commandBus.Publish("SelectFolder");

            // assert
            Assert.AreEqual("some path", viewMock.Object.Model.Folder);
        }
    }
}
