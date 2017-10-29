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
        private MenuPresenter menuPresenter;
        private Mock<IMenuView> viewMock;
        private MainModel mainModel;
        private Mock<ICommandBus> commandBusMock;

        [SetUp]
        public void SetUp()
        {
            this.viewMock = new Mock<IMenuView>();
            this.mainModel = new MainModel();
            this.commandBusMock = new Mock<ICommandBus>();
            this.menuPresenter = new MenuPresenter(
                this.viewMock.Object,
                this.mainModel,
                this.commandBusMock.Object);

            this.viewMock.Raise(v => v.Load += null, EventArgs.Empty);
        }

        [Test]
        public void FileOpenClick_ShowsMainWindow()
        {
            this.viewMock.Raise(v => v.FileOpenClick += null, EventArgs.Empty);
            this.viewMock.Verify(v => v.ShowMainWindow());
        }

        [Test]
        public void FileOpenClick_PublishesCommand()
        {
            this.viewMock.Raise(v => v.FileOpenClick += null, EventArgs.Empty);
            this.commandBusMock.Verify(v => v.Publish("SelectFolder"));
        }

        [Test]
        public void OpenRecentFileClick_ShowsMainWindow()
        {
            this.viewMock.Raise(v => v.OpenRecentFileClick += null, new RecentFileEventArgs("file"));
            this.viewMock.Verify(v => v.ShowMainWindow());
        }

        [Test]
        public void OpenRecentFileClick_SetsModelFolder()
        {
            this.viewMock.Raise(v => v.OpenRecentFileClick += null, new RecentFileEventArgs("file"));
            Assert.AreEqual("file", this.mainModel.Folder);
        }
    }
}
