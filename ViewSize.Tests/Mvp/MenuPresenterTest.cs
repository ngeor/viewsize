// <copyright file="MenuPresenterTest.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;
using CRLFLabs.ViewSize.Mvp;
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
            protected Mock<IMainModel> mainModelMock;
            protected Mock<IFolderChooserAction> folderChooserActionMock;
#pragma warning restore SA1401 // Fields must be private

            [SetUp]
            public void SetUp()
            {
                viewMock = new Mock<IMenuView>();

                mainModelMock = new Mock<IMainModel>();
                mainModelMock.SetupProperty(m => m.Folder)
                    .SetupProperty(m => m.RecentFolders)
                    .Object.RecentFolders = new string[0];

                folderChooserActionMock = new Mock<IFolderChooserAction>();

                menuPresenter = new MenuPresenter(
                    viewMock.Object,
                    mainModelMock.Object,
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
                viewMock.Verify(v => v.SetRecentFolders(new string[0]));
            }

            [Test]
            public void WhenRecentFoldersExist_AddRecentFolderIsCalled()
            {
                // arrange
                mainModelMock.Object.RecentFolders = new[] { "a", "b" };

                // act
                viewMock.Raise(v => v.Load += null, EventArgs.Empty);

                // assert
                viewMock.Verify(v => v.SetRecentFolders(new[] { "a", "b" }));
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
                Assert.AreEqual("file", mainModelMock.Object.Folder);
            }
        }
    }
}
