// <copyright file="RecentFoldersPresenterTest.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;
using CRLFLabs.ViewSize.Mvp;
using Moq;
using NUnit.Framework;

namespace ViewSize.Tests.Mvp
{
    public class RecentFoldersPresenterTest
    {
        public class Base
        {
#pragma warning disable SA1401 // Fields must be private
            protected RecentFoldersPresenter presenter;
            protected Mock<IRecentFoldersView> viewMock;
            protected Mock<IMainModel> mainModelMock;
#pragma warning restore SA1401 // Fields must be private

            [SetUp]
            public void SetUp()
            {
                viewMock = new Mock<IRecentFoldersView>();

                mainModelMock = new Mock<IMainModel>();
                mainModelMock.SetupProperty(m => m.Folder)
                    .SetupProperty(m => m.RecentFolders)
                    .Object.RecentFolders = new string[0];

                presenter = new RecentFoldersPresenter(
                    viewMock.Object,
                    mainModelMock.Object);

                viewMock.Raise(v => v.Load += null, EventArgs.Empty);
            }
        }

        public class Ctor : Base
        {
        }

        public class Load : Base
        {
            [Test]
            public void SetsRecentFoldersOnLoadEvenIfEmpty()
            {
                viewMock.Verify(v => v.SetRecentFolders(new string[0]));
            }

            [Test]
            public void SetRecentFoldersOnLoad()
            {
                // arrange
                mainModelMock.Object.RecentFolders = new[] { "a", "b" };

                // act
                viewMock.Raise(v => v.Load += null, EventArgs.Empty);

                // assert
                viewMock.Verify(v => v.SetRecentFolders(new[] { "a", "b" }));
            }
        }
    }
}
