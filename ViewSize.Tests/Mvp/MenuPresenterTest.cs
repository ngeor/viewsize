// <copyright file="MenuPresenterTest.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;
using CRLFLabs.ViewSize.Mvp;
using CRLFLabs.ViewSize.Settings;
using Moq;
using NUnit.Framework;

namespace ViewSize.Tests.Mvp
{
    public class MenuPresenterTest
    {
        private MenuPresenter menuPresenter;
        private Mock<IMenuView> viewMock;
        private MainModel mainModel;
        private Mock<ICommandBus> commandBusMock;
        private Mock<ISettingsManager> settingsManagerMock;

        [SetUp]
        public void SetUp()
        {
            viewMock = new Mock<IMenuView>();
            mainModel = new MainModel();
            commandBusMock = new Mock<ICommandBus>();
            settingsManagerMock = new Mock<ISettingsManager>();
            menuPresenter = new MenuPresenter(
                viewMock.Object,
                mainModel,
                commandBusMock.Object,
                settingsManagerMock.Object);

            viewMock.Raise(v => v.Load += null, EventArgs.Empty);
        }

        [Test]
        public void Load_WhenNoRecentFoldersExist_AddRecentFolderIsNotCalled()
        {
            viewMock.Verify(v => v.AddRecentFolder(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void Load_WhenRecentFoldersExist_AddRecentFolderIsCalled()
        {
            // arrange
            var settings = new Settings
            {
                RecentFolders = new[] { "a", "b" }
            };
            settingsManagerMock.SetupGet(m => m.Settings).Returns(settings);

            // act
            viewMock.Raise(v => v.Load += null, EventArgs.Empty);

            // assert
            viewMock.Verify(v => v.AddRecentFolder("a"));
            viewMock.Verify(v => v.AddRecentFolder("b"));
        }

        [Test]
        public void FileOpenClick_ShowsMainWindow()
        {
            viewMock.Raise(v => v.FileOpenClick += null, EventArgs.Empty);
            viewMock.Verify(v => v.ShowMainWindow());
        }

        [Test]
        public void FileOpenClick_PublishesCommand()
        {
            viewMock.Raise(v => v.FileOpenClick += null, EventArgs.Empty);
            commandBusMock.Verify(v => v.Publish("SelectFolder"));
        }

        [Test]
        public void OpenRecentFileClick_ShowsMainWindow()
        {
            viewMock.Raise(v => v.OpenRecentFileClick += null, new RecentFileEventArgs("file"));
            viewMock.Verify(v => v.ShowMainWindow());
        }

        [Test]
        public void OpenRecentFileClick_SetsModelFolder()
        {
            viewMock.Raise(v => v.OpenRecentFileClick += null, new RecentFileEventArgs("file"));
            Assert.AreEqual("file", mainModel.Folder);
        }
    }
}
