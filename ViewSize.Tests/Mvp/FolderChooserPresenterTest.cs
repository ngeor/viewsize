// <copyright file="FolderChooserPresenterTest.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;
using CRLFLabs.ViewSize.Mvp;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace ViewSize.Tests.Mvp
{
    [TestFixture]
    public class FolderChooserPresenterTest
    {
        public class Base
        {
#pragma warning disable SA1401 // Fields must be private
            protected FolderChooserPresenter presenter;
            protected Mock<IFolderChooserView> viewMock;
            protected MainModel model;
            protected Mock<IFolderChooserAction> folderChooserActionMock;
#pragma warning restore SA1401 // Fields must be private

            [SetUp]
            public void SetUp()
            {
                model = new MainModel();
                viewMock = new Mock<IFolderChooserView>();
                viewMock.SetupProperty(v => v.Folder);
                viewMock.SetupProperty(v => v.Model);
                folderChooserActionMock = new Mock<IFolderChooserAction>();

                presenter = new FolderChooserPresenter(
                    viewMock.Object,
                    model,
                    folderChooserActionMock.Object);

                viewMock.Raise(v => v.Load += null, EventArgs.Empty);
            }
        }

        public class Ctor : Base
        {
        }

        public class OnSelectFolderClick : Base
        {
            [SetUp]
            public new void SetUp()
            {
                // arrange
                viewMock.Setup(v => v.SelectFolder()).Returns("some path");

                // act
                viewMock.Raise(v => v.OnSelectFolderClick += null, EventArgs.Empty);
            }

            [Test]
            public void CallsAction()
            {
                folderChooserActionMock.Verify(v => v.SelectFolder());
            }
        }
    }
}
