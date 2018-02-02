// <copyright file="ModelSettingsFolderSyncTest.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using CRLFLabs.ViewSize.Mvp;
using CRLFLabs.ViewSize.Settings;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System.ComponentModel;

namespace ViewSize.Tests.Mvp
{
    public class ModelSettingsFolderSyncTest
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
                modelMock.SetupProperty(m => m.Folder);
                settings.SelectedFolder = "/tmp";

                // act
                new ModelSettingsFolderSync(modelMock.Object, settingsManagerMock.Object);

                // assert
                modelMock.Object.Folder.Should().Be("/tmp");
            }
        }

        public class FolderChange : Base
        {
            [Test]
            public void SetsSettingsWhenModelChanges()
            {
                // arrange
                var modelMock = new Mock<IMainModel>();
                modelMock.SetupProperty(m => m.Folder);
                settings.SelectedFolder = "/tmp";
                new ModelSettingsFolderSync(modelMock.Object, settingsManagerMock.Object);

                // act
                modelMock.Object.Folder = "something else";
                modelMock.Raise(m => m.PropertyChanged += null, new PropertyChangedEventArgs(MainModel.FolderPropertyName));

                // assert
                settings.SelectedFolder.Should().Be("something else");
            }
        }
    }
}
