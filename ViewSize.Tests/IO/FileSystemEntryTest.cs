// <copyright file="FileSystemEntryTest.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System.Linq;
using CRLFLabs.ViewSize.IO;
using NUnit.Framework;

namespace ViewSize.Tests.IO
{
    [TestFixture]
    public class FileSystemEntryTest
    {
        public class Ancestors
        {
            [Test]
            public void RootShouldBeEmpty()
            {
                // arrange
                var entry = new FileSystemEntry("entry", null);

                // act
                var ancestors = entry.Ancestors();

                // assert
                Assert.IsFalse(ancestors.Any());
            }

            [Test]
            public void SingleAncestorShouldHaveOne()
            {
                // arrange
                var parent = new FileSystemEntry("parent", null);
                var entry = new FileSystemEntry("child", parent);

                // act
                var ancestors = entry.Ancestors();

                // assert
                CollectionAssert.AreEqual(new[] { parent }, ancestors);
            }

            [Test]
            public void TwoAncestorsShouldHaveNearestLast()
            {
                // arrange
                var grandParent = new FileSystemEntry("grand parent", null);
                var parent = new FileSystemEntry("parent", grandParent);
                var entry = new FileSystemEntry("child", parent);

                // act
                var ancestors = entry.Ancestors();

                // assert
                CollectionAssert.AreEqual(new[] { grandParent, parent }, ancestors);
            }
        }
    }
}
