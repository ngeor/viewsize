// <copyright file="FolderChooserActionTest.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using CRLFLabs.ViewSize.Mvp;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace ViewSize.Tests.Mvp
{
    [TestFixture]
    public class FolderChooserActionTest
    {
        public class Base
        {
#pragma warning disable SA1401 // Fields must be private
            protected FolderChooserAction action;
            protected Mock<IFolderChooserView> viewMock;
            protected Mock<IMainModel> modelMock;
#pragma warning restore SA1401 // Fields must be private

            [SetUp]
            public void SetUp()
            {
                modelMock = new Mock<IMainModel>();
                modelMock.SetupProperty(m => m.Folder);
                viewMock = new Mock<IFolderChooserView>();
                viewMock.SetupProperty(v => v.Folder);
                viewMock.SetupProperty(v => v.Model);

                action = new FolderChooserAction(
                    modelMock.Object,
                    viewMock.Object);
            }
        }

        public class Ctor : Base
        {
        }

        public class OnSelectFolderClick_WhenViewReturnsFolder : Base
        {
            [SetUp]
            public new void SetUp()
            {
                // arrange
                modelMock.Object.Folder = "/tmp";
                viewMock.Setup(v => v.SelectFolder()).Returns("some path");

                // act
                action.SelectFolder();
            }

            [Test]
            public void UpdatesModelFolder()
            {
                Assert.AreEqual("some path", modelMock.Object.Folder);
            }
        }

        public class OnSelectFolderClick_WhenViewReturnsNull : Base
        {
            [SetUp]
            public new void SetUp()
            {
                // arrange
                modelMock.Object.Folder = "/tmp";
                viewMock.Setup(v => v.SelectFolder()).Returns((string)null);

                // act
                action.SelectFolder();
            }

            [Test]
            public void DoesNotUpdateModelFolder()
            {
                Assert.AreEqual("/tmp", modelMock.Object.Folder);
            }
        }
    }
}
