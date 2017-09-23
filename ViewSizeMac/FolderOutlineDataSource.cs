
using AppKit;
using Foundation;
using System.Linq;
using System.Collections.Generic;
using CRLFLabs.ViewSize;

namespace ViewSizeMac
{
    public class FolderOutlineDataSource : NSOutlineViewDataSource
    {
        public FolderOutlineDataSource(IList<FileSystemEntry> topLevelFolders)
        {
            TopLevelFolders = topLevelFolders;
        }

        private IList<FileSystemEntry> TopLevelFolders { get; }

        public override System.nint GetChildrenCount(NSOutlineView outlineView, NSObject item)
        {
            return ListOf(item).Count;
        }

        public override NSObject GetChild(NSOutlineView outlineView, System.nint childIndex, NSObject item)
        {
            return ListOf(item)[(int)childIndex];
        }

        public override bool ItemExpandable(NSOutlineView outlineView, NSObject item)
        {
            return ListOf(item).Any();
        }

        private IList<FileSystemEntry> ListOf(NSObject item)
        {
            if (item == null)
            {
                return TopLevelFolders;
            }
            else
            {
                return ((FileSystemEntry)item).Children;
            }
        }
    }
}
