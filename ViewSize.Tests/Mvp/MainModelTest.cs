// <copyright file="MainModelTest.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using CRLFLabs.ViewSize.Mvp;
using FluentAssertions;
using NUnit.Framework;

namespace ViewSize.Tests.Mvp
{
    [TestFixture]
    public class MainModelTest
    {
        public class Base
        {
        }

        public class RecentFolders : Base
        {
            [Test]
            public void ShouldNotBeNull()
            {
                var model = new MainModel();
                model.RecentFolders.Should().NotBeNull();
            }

            [Test]
            public void CannotSetToNull()
            {
                var model = new MainModel();
                Assert.Throws<ArgumentOutOfRangeException>(() =>
                {
                    model.RecentFolders = null;
                });
            }

            [Test]
            public void CanSetToOtherList()
            {
                var model = new MainModel();
                var propertyNames = new List<string>();
                model.PropertyChanged += (_, args) =>
                {
                    propertyNames.Add(args.PropertyName);
                };

                // act
                model.RecentFolders = new[] { "/tmp" };

                // assert
                propertyNames.Should().Equal("RecentFolders");
                model.RecentFolders.Should().Equal("/tmp");
            }

            [Test]
            public void SettingFolderUpdatesRecentFolders()
            {
                var model = new MainModel();
                var propertyNames = new List<string>();
                model.PropertyChanged += (_, args) =>
                {
                    propertyNames.Add(args.PropertyName);
                };

                // act
                model.Folder = "/tmp";

                // assert
                propertyNames.Should().Equal("Folder", "RecentFolders");
                model.RecentFolders.Should().Equal("/tmp");
            }

            [Test]
            public void SettingFolderPutsNewFolderFirst()
            {
                var model = new MainModel
                {
                    Folder = "/var/log",
                    RecentFolders = new[] { "/var/log" }
                };

                var propertyNames = new List<string>();
                model.PropertyChanged += (_, args) =>
                {
                    propertyNames.Add(args.PropertyName);
                };

                // act
                model.Folder = "/tmp";

                // assert
                propertyNames.Should().Equal("Folder", "RecentFolders");
                model.RecentFolders.Should().Equal("/tmp", "/var/log");
            }

            [Test]
            public void SettingFolderRemovesDuplicates()
            {
                var model = new MainModel
                {
                    Folder = "/var/log",
                    RecentFolders = new[] { "/var/log", "/var/log", "/var/lib", "/var/usr", "/var/lib" }
                };

                var propertyNames = new List<string>();
                model.PropertyChanged += (_, args) =>
                {
                    propertyNames.Add(args.PropertyName);
                };

                // act
                model.Folder = "/var/usr";

                // assert
                propertyNames.Should().Equal("Folder", "RecentFolders");
                model.RecentFolders.Should().Equal("/var/usr", "/var/log", "/var/lib");
            }

            [Test]
            public void SettingFolderKeepsMaxTenFolders()
            {
                var model = new MainModel
                {
                    Folder = "/var/log",
                    RecentFolders = Enumerable.Range(1, 10).Select(i => i.ToString()).ToArray()
                };

                var propertyNames = new List<string>();
                model.PropertyChanged += (_, args) =>
                {
                    propertyNames.Add(args.PropertyName);
                };

                // act
                model.Folder = "/var/usr";

                // assert
                propertyNames.Should().Equal("Folder", "RecentFolders");
                model.RecentFolders.Should().Equal("/var/usr", "1", "2", "3", "4", "5", "6", "7", "8", "9");
            }
        }
    }
}
