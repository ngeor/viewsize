using System;
using CRLFLabs.ViewSize;
using CRLFLabs.ViewSize.Drawing;
using CRLFLabs.ViewSize.IO;
using CRLFLabs.ViewSize.TreeMap;
using Moq;
using NUnit.Framework;

namespace ViewSize.Tests.TreeMap
{
    [TestFixture]
    public class TreeMapDataSourceTest
    {
        [Test]
        public void Create()
        {
            // arrange
            var folderScanner = Mock.Of<IFileSystemEntryContainer>();
            var topLevelEntry = new FileSystemEntry("root", folderScanner);
            var childEntry = new FileSystemEntry("child", topLevelEntry);
            var topLevelEntries = Lists.Of(topLevelEntry);

            // act
            TreeMapDataSource dataSource = new TreeMapDataSource(topLevelEntries);

            // assert
            var dataSourceChildren = dataSource.Children;
            Assert.IsNotNull(dataSourceChildren, "children of datasource");
            Assert.AreEqual(1, dataSourceChildren.Count, "children of datasource");

            var dataSourceTopLevelEntry = dataSourceChildren[0];
            Assert.IsNotNull(dataSourceTopLevelEntry, "first top level entry");
            Assert.AreEqual("root", dataSourceTopLevelEntry.Path, "first top level entry Path");
            Assert.AreEqual(dataSource, dataSourceChildren[0].Parent, "first top level entry Parent");

            var dataSourceSecondLevelEntry = dataSourceTopLevelEntry.Children[0];
            Assert.IsNotNull(dataSourceSecondLevelEntry, "first child entry");
            Assert.AreEqual("child", dataSourceSecondLevelEntry.Path, "first child entry Path");
            Assert.AreEqual(dataSourceTopLevelEntry, dataSourceSecondLevelEntry.Parent, "first child entry Parent");
        }
    }
}
