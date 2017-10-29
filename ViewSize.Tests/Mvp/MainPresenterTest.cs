// <copyright file="MainPresenterTest.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;
using CRLFLabs.ViewSize.IO;
using CRLFLabs.ViewSize.Mvp;
using Moq;
using NUnit.Framework;

namespace ViewSize.Tests.Mvp
{
    [TestFixture]
    public class MainPresenterTest
    {
        private MainPresenter presenter;
        private Mock<IMainView> viewMock;
        private Mock<IFolderScanner> folderScannerMock;
        private Mock<IFileUtils> fileUtilsMock;
        private MainModel model;

        [SetUp]
        public void SetUp()
        {
            this.viewMock = new Mock<IMainView>();
            this.folderScannerMock = new Mock<IFolderScanner>();
            this.fileUtilsMock = new Mock<IFileUtils>();
            this.presenter = new MainPresenter(
                this.viewMock.Object,
                this.model = new MainModel(),
                this.folderScannerMock.Object,
                this.fileUtilsMock.Object);

            this.viewMock.Raise(v => v.Load += null, EventArgs.Empty);
        }

        [Test]
        public void OnBeginScan_NoFolderSelected_ShouldShowError()
        {
            // arrange
            this.model.Folder = string.Empty;

            // act
            this.viewMock.Raise(v => v.OnBeginScanClick += null, EventArgs.Empty);

            // assert
            this.viewMock.Verify(v => v.ShowError("No folder selected!"));
        }

        [Test]
        public void OnBeginScan_SelectedFolderDoesNotExist_ShouldShowError()
        {
            // arrange
            this.model.Folder = "test";
            this.fileUtilsMock.Setup(f => f.IsDirectory("test")).Returns(false);

            // act
            this.viewMock.Raise(v => v.OnBeginScanClick += null, EventArgs.Empty);

            // assert
            this.viewMock.Verify(v => v.ShowError("Folder 'test' does not exist!"));
        }

        [Test]
        public void UpOneLevel_NoSelected_DoesNothing()
        {
            // arrange

            // act
            this.viewMock.Raise(v => v.UpOneLevelClick += null, EventArgs.Empty);

            // assert
            Assert.IsNull(this.model.Selected);
        }

        [Test]
        public void UpOneLevel_NoParent_DoesNothing()
        {
            // arrange
            var selected = new FileSystemEntry("test", null);
            this.model.Selected = selected;

            // act
            this.viewMock.Raise(v => v.UpOneLevelClick += null, EventArgs.Empty);

            // assert
            Assert.AreSame(selected, this.model.Selected);
        }

        [Test]
        public void UpOneLevel_WithParent_SelectsParent()
        {
            // arrange
            var parent = new FileSystemEntry("test", null);
            var selected = new FileSystemEntry("child", parent);
            this.model.Selected = selected;

            // act
            this.viewMock.Raise(v => v.UpOneLevelClick += null, EventArgs.Empty);

            // assert
            Assert.AreSame(parent, this.model.Selected);
        }
    }
}
