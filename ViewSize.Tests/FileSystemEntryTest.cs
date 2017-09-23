using System;
using System.Linq;
using CRLFLabs.ViewSize;
using CRLFLabs.ViewSize.TreeMap;
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
            var entry = new FileSystemEntry("entry");

            // act
            var ancestors = entry.Ancestors();

            // assert
            Assert.IsFalse(ancestors.Any());
        }

        [Test]
        public void Ancestors_SingleAncestor_ShouldHaveOne()
        {
            // arrange
            var parent = new FileSystemEntry("parent");
            var entry = new FileSystemEntry("child")
            {
                Parent = parent
            };

            // act
            var ancestors = entry.Ancestors();

            // assert
            CollectionAssert.AreEqual(new[] { parent }, ancestors);
        }

        [Test]
        public void Ancestor_UnderTreeMapDataSource_ShouldBeEmpty()
        {
            // arrange
            var parent = new TreeMapDataSource();
            var entry = new FileSystemEntry("entry")
            {
                Parent = parent
            };

            // act
            var ancestors = entry.Ancestors();

            // assert
            Assert.IsFalse(ancestors.Any());
        }

        [Test]
        public void Ancestors_TwoAncestors_ShouldHaveNearestLast()
        {
            // arrange
            var grandParent = new FileSystemEntry("grand parent");
            var parent = new FileSystemEntry("parent")
            {
                Parent = grandParent
            };
            var entry = new FileSystemEntry("child")
            {
                Parent = parent
            };

            // act
            var ancestors = entry.Ancestors();

            // assert
            CollectionAssert.AreEqual(new[] { grandParent, parent }, ancestors);
        }
    }
}
