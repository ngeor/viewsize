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
            viewMock = new Mock<IMainView>();
            folderScannerMock = new Mock<IFolderScanner>();
            fileUtilsMock = new Mock<IFileUtils>();
            presenter = new MainPresenter(
                viewMock.Object,
                model = new MainModel(),
                folderScannerMock.Object,
                fileUtilsMock.Object);

            viewMock.Raise(v => v.Load += null, EventArgs.Empty);
        }

        [Test]
        public void OnBeginScan_NoFolderSelected_ShouldShowError()
        {
            // arrange
            model.Folder = string.Empty;

            // act
            viewMock.Raise(v => v.OnBeginScanClick += null, EventArgs.Empty);

            // assert
            viewMock.Verify(v => v.ShowError("No folder selected!"));
        }

        [Test]
        public void OnBeginScan_SelectedFolderDoesNotExist_ShouldShowError()
        {
            // arrange
            model.Folder = "test";
            fileUtilsMock.Setup(f => f.IsDirectory("test")).Returns(false);

            // act
            viewMock.Raise(v => v.OnBeginScanClick += null, EventArgs.Empty);

            // assert
            viewMock.Verify(v => v.ShowError("Folder 'test' does not exist!"));
        }

        [Test]
        public void UpOneLevel_NoSelected_DoesNothing()
        {
            // arrange

            // act
            viewMock.Raise(v => v.UpOneLevelClick += null, EventArgs.Empty);

            // assert
            Assert.IsNull(model.Selected);
        }

        [Test]
        public void UpOneLevel_NoParent_DoesNothing()
        {
            // arrange
            var selected = new FileSystemEntry("test", null);
            model.Selected = selected;

            // act
            viewMock.Raise(v => v.UpOneLevelClick += null, EventArgs.Empty);

            // assert
            Assert.AreSame(selected, model.Selected);
        }

        [Test]
        public void UpOneLevel_WithParent_SelectsParent()
        {
            // arrange
            var parent = new FileSystemEntry("test", null);
            var selected = new FileSystemEntry("child", parent);
            model.Selected = selected;

            // act
            viewMock.Raise(v => v.UpOneLevelClick += null, EventArgs.Empty);

            // assert
            Assert.AreSame(parent, model.Selected);
        }
    }
}
