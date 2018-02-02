// <copyright file="ModelSettingsRecentFoldersSyncTest.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System.ComponentModel;
using CRLFLabs.ViewSize.Mvp;
using CRLFLabs.ViewSize.Settings;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace ViewSize.Tests.Mvp
{
    public class ModelSettingsRecentFoldersSyncTest
    {
        public class Base
        {
#pragma warning disable SA1401 // Fields must be private
            protected Mock<ISettingsManager> settingsManagerMock;
            protected Settings settings;
#pragma warning restore SA1401 // Fields must be private

            [SetUp]
            public void SetUp()
            {
                settings = new Settings();
                settingsManagerMock = new Mock<ISettingsManager>();
                settingsManagerMock.SetupGet(m => m.Settings).Returns(settings);
            }
        }

        public class Ctor : Base
        {
            [Test]
            public void SetsModelFromSettings()
            {
                // arrange
                var modelMock = new Mock<IMainModel>();
                modelMock.SetupProperty(m => m.RecentFolders);
                settings.RecentFolders = new[] { "/tmp" };

                // act
                new ModelSettingsRecentFoldersSync(modelMock.Object, settingsManagerMock.Object);

                // assert
                modelMock.Object.RecentFolders.Should().Equal("/tmp");
            }

            [Test]
            public void ConvertsNullRecentFoldersToEmpty()
            {
                // arrange
                var modelMock = new Mock<IMainModel>();
                modelMock.SetupProperty(m => m.RecentFolders);

                // act
                new ModelSettingsRecentFoldersSync(modelMock.Object, settingsManagerMock.Object);

                // assert
                modelMock.Object.RecentFolders.Should().NotBeNull();
                modelMock.Object.RecentFolders.Should().BeEmpty();
            }
        }

        public class FolderChange : Base
        {
            [Test]
            public void SetsSettingsWhenModelChanges()
            {
                // arrange
                var modelMock = new Mock<IMainModel>();
                modelMock.SetupProperty(m => m.RecentFolders);
                settings.RecentFolders = new[] { "/tmp" };
                new ModelSettingsRecentFoldersSync(modelMock.Object, settingsManagerMock.Object);

                // act
                modelMock.Object.RecentFolders = new[] { "something else" };
                modelMock.Raise(m => m.PropertyChanged += null, new PropertyChangedEventArgs(MainModel.RecentFoldersPropertyName));

                // assert
                settings.RecentFolders.Should().Equal("something else");
            }
        }
    }
}
