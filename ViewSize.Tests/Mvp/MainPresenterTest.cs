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
        public class Base
        {
#pragma warning disable SA1401 // Fields must be private
            protected MainPresenter presenter;
            protected Mock<IMainView> viewMock;
            protected Mock<IFolderScanner> folderScannerMock;
            protected Mock<IFileUtils> fileUtilsMock;
            protected MainModel model;
#pragma warning restore SA1401 // Fields must be private

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
        }

        public class OnBeginScan : Base
        {
            [Test]
            public void NoFolderSelected_ShouldShowError()
            {
                // arrange
                model.Folder = string.Empty;

                // act
                viewMock.Raise(v => v.OnBeginScanClick += null, EventArgs.Empty);

                // assert
                viewMock.Verify(v => v.ShowError("No folder selected!"));
            }

            [Test]
            public void SelectedFolderDoesNotExist_ShouldShowError()
            {
                // arrange
                model.Folder = "test";
                fileUtilsMock.Setup(f => f.IsDirectory("test")).Returns(false);

                // act
                viewMock.Raise(v => v.OnBeginScanClick += null, EventArgs.Empty);

                // assert
                viewMock.Verify(v => v.ShowError("Folder 'test' does not exist!"));
            }
        }

        public class UpOneLevel : Base
        {
            [Test]
            public void NoSelected_DoesNothing()
            {
                // arrange

                // act
                viewMock.Raise(v => v.UpOneLevelClick += null, EventArgs.Empty);

                // assert
                Assert.IsNull(model.Selected);
            }

            [Test]
            public void NoParent_DoesNothing()
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
            public void WithParent_SelectsParent()
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
}
