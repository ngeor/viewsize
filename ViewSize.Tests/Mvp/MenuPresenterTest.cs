// <copyright file="MenuPresenterTest.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;
using CRLFLabs.ViewSize.IoC;
using CRLFLabs.ViewSize.Mvp;
using CRLFLabs.ViewSize.Settings;
using Moq;
using NUnit.Framework;

namespace ViewSize.Tests.Mvp
{
    public class MenuPresenterTest
    {
        public class Base
        {
#pragma warning disable SA1401 // Fields must be private
            protected MenuPresenter menuPresenter;
            protected Mock<IMenuView> viewMock;
            protected MainModel mainModel;
            protected Mock<IFolderChooserAction> folderChooserActionMock;
            protected Mock<ISettingsManager> settingsManagerMock;
#pragma warning restore SA1401 // Fields must be private

            [SetUp]
            public void SetUp()
            {
                viewMock = new Mock<IMenuView>();
                mainModel = new MainModel();
                settingsManagerMock = new Mock<ISettingsManager>();
                folderChooserActionMock = new Mock<IFolderChooserAction>();

                menuPresenter = new MenuPresenter(
                    viewMock.Object,
                    mainModel,
                    settingsManagerMock.Object,
                    folderChooserActionMock.Object);

                viewMock.Raise(v => v.Load += null, EventArgs.Empty);
            }
        }

        public class Ctor : Base
        {
        }

        public class Load : Base
        {
            [Test]
            public void WhenNoRecentFoldersExist_AddRecentFolderIsNotCalled()
            {
                viewMock.Verify(v => v.AddRecentFolder(It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
            }

            [Test]
            public void WhenRecentFoldersExist_AddRecentFolderIsCalled()
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
                viewMock.Verify(v => v.AddRecentFolder("a", false));
                viewMock.Verify(v => v.AddRecentFolder("b", false));
            }
        }

        public class FileOpenClick : Base
        {
            [SetUp]
            public new void SetUp()
            {
                // act
                viewMock.Raise(v => v.FileOpenClick += null, EventArgs.Empty);
            }

            [Test]
            public void ShowsMainWindow()
            {
                viewMock.Verify(v => v.ShowMainWindow());
            }

            [Test]
            public void PublishesCommand()
            {
                folderChooserActionMock.Verify(p => p.SelectFolder());
            }
        }

        public class OpenRecentFileClick : Base
        {
            [SetUp]
            public new void SetUp()
            {
                // act
                viewMock.Raise(v => v.OpenRecentFileClick += null, new RecentFileEventArgs("file"));
            }

            [Test]
            public void ShowsMainWindow()
            {
                viewMock.Verify(v => v.ShowMainWindow());
            }

            [Test]
            public void SetsModelFolder()
            {
                Assert.AreEqual("file", mainModel.Folder);
            }
        }
    }
}
