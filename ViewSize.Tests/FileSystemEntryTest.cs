using System;
using System.Linq;
using CRLFLabs.ViewSize;
using CRLFLabs.ViewSize.Drawing;
using CRLFLabs.ViewSize.TreeMap;
using Moq;
using NUnit.Framework;

namespace ViewSize.Tests
{
    [TestFixture]
    public class FileSystemEntryTest
    {
        [Test]
        public void Ancestors_Root_ShouldBeEmpty()
        {
            // arrange
            var entry = new FileSystemEntry("entry", Mock.Of<IFileSystemEntryContainer>());

            // act
            var ancestors = entry.Ancestors();

            // assert
            Assert.IsFalse(ancestors.Any());
        }

        [Test]
        public void Ancestors_SingleAncestor_ShouldHaveOne()
        {
            // arrange
            var parent = new FileSystemEntry("parent", Mock.Of<IFileSystemEntryContainer>());
            var entry = new FileSystemEntry("child", parent);

            // act
            var ancestors = entry.Ancestors();

            // assert
            CollectionAssert.AreEqual(new[] { parent }, ancestors);
        }

        [Test]
        public void Ancestor_UnderTreeMapDataSource_ShouldBeEmpty()
        {
            // arrange
            var parent = new TreeMapDataSource(Enumerable.Empty<FileSystemEntry>(), default(RectangleD));
            var entry = new FileSystemEntry("entry", parent);

            // act
            var ancestors = entry.Ancestors();

            // assert
            Assert.IsFalse(ancestors.Any());
        }

        [Test]
        public void Ancestors_TwoAncestors_ShouldHaveNearestLast()
        {
            // arrange
            var grandParent = new FileSystemEntry("grand parent", Mock.Of<IFileSystemEntryContainer>());
            var parent = new FileSystemEntry("parent", grandParent);
            var entry = new FileSystemEntry("child", parent);

            // act
            var ancestors = entry.Ancestors();

            // assert
            CollectionAssert.AreEqual(new[] { grandParent, parent }, ancestors);
        }
    }
}
