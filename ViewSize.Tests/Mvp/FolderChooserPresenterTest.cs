// <copyright file="FolderChooserPresenterTest.cs" company="CRLFLabs">
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
    [TestFixture]
    public class FolderChooserPresenterTest
    {
        public class Base
        {
#pragma warning disable SA1401 // Fields must be private
            protected FolderChooserPresenter presenter;
            protected Mock<IFolderChooserView> viewMock;
            protected MainModel model;
            protected Mock<ISettingsManager> settingsManagerMock;
            protected Settings settings;
            protected Mock<IResolver> resolverMock;
            protected Mock<IMenuPresenter> menuPresenterMock;
#pragma warning restore SA1401 // Fields must be private

            [SetUp]
            public void SetUp()
            {
                model = new MainModel();
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
                viewMock.SetupProperty(v => v.Folder);
                viewMock.SetupProperty(v => v.Model);
                resolverMock = new Mock<IResolver>();

                menuPresenterMock = new Mock<IMenuPresenter>();
                resolverMock.Setup(r => r.Resolve<IMenuPresenter>()).Returns(menuPresenterMock.Object);

                presenter = new FolderChooserPresenter(
                    viewMock.Object,
                    model,
                    settingsManagerMock.Object,
                    resolverMock.Object);

                viewMock.Raise(v => v.Load += null, EventArgs.Empty);
            }
        }

        public class Ctor : Base
        {
            [Test]
            public void RegistersAsIFolderChooserPresenter()
            {
                resolverMock.Verify(v => v.MapExistingInstance(typeof(IFolderChooserPresenter), presenter));
            }
        }

        public class OnSelectFolderClick_WhenViewReturnsFolder : Base
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
            public void UpdatesModelFolder()
            {
                Assert.AreEqual("some path", model.Folder);
            }
        }

        public class OnSelectFolderClick_WhenViewReturnsNull : Base
        {
            [SetUp]
            public new void SetUp()
            {
                // arrange
                viewMock.Setup(v => v.SelectFolder()).Returns((string)null);

                // act
                viewMock.Raise(v => v.OnSelectFolderClick += null, EventArgs.Empty);
            }

            [Test]
            public void DoesNotUpdateModelFolder()
            {
                Assert.AreEqual("/tmp", model.Folder);
            }
        }

        public class ModelUpdated : Base
        {
            [SetUp]
            public new void SetUp()
            {
                model.Folder = "some path";
            }

            [Test]
            public void UpdatesViewFolder()
            {
                Assert.AreEqual("some path", viewMock.Object.Folder);
            }

            [Test]
            public void UpdatesSettingsSelectedFolder()
            {
                Assert.AreEqual("some path", settings.SelectedFolder);
            }

            [Test]
            public void UpdatesSettingsRecentFolders()
            {
                var expected = new[]
                {
                        "some path",
                        "/tmp"
                    };

                CollectionAssert.AreEqual(expected, settings.RecentFolders);
            }

            [Test]
            public void NotifiesMenuPresenter()
            {
                menuPresenterMock.Verify(v => v.AddRecentFolder("some path"));
            }
        }

        public class ModelUpdated_ViewSupportsNativeRecentFolders : Base
        {
            [SetUp]
            public new void SetUp()
            {
                viewMock.SetupGet(v => v.HasNativeRecentFolders).Returns(true);
                model.Folder = "some path";
            }

            [Test]
            public void DoesNotUpdateSettingsRecentFolders()
            {
                var expected = new[]
                {
                        "/tmp"
                    };

                CollectionAssert.AreEqual(expected, settings.RecentFolders);
            }

            [Test]
            public void DoesNotNotifyMenuPresenter()
            {
                menuPresenterMock.Verify(v => v.AddRecentFolder("some path"), Times.Never);
            }
        }
    }
}
