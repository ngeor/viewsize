
using AppKit;
using Foundation;
using CRLFLabs.ViewSize;
using System.Collections.Generic;
using System.Linq;

namespace ViewSizeMac
{
    class FolderViewModel : NSObject
    {
        public FolderViewModel(FileEntry fileEntry)
        {
            FileEntry = fileEntry;
        }

        private FileEntry FileEntry { get; }

        public List<FolderViewModel> Children => FileEntry.Children.Select(c => new FolderViewModel(c)).ToList();
    }

    class FolderOutlineDataSource : NSOutlineViewDataSource
    {
        public FolderOutlineDataSource(FolderViewModel root)
        {
            Root = root;
        }

        private FolderViewModel Root { get; }

        public override System.nint GetChildrenCount(NSOutlineView outlineView, NSObject item)
        {
            if (item == null)
            {
                return Root == null ? 0 : 1;
            }
            else
            {
                return ((FolderViewModel)item).Children.Count;
            }
        }

        public override NSObject GetChild(NSOutlineView outlineView, System.nint childIndex, NSObject item)
        {
            if (item == null)
            {
                return Root;
            }
            else
            {
                return ((FolderViewModel)item).Children[(int)childIndex];
            }
        }

        public override bool ItemExpandable(NSOutlineView outlineView, NSObject item)
        {
            if (item == null)
            {
                return Root.Children.Any();
            }
            else
            {
                return ((FolderViewModel)item).Children.Any();
            }
        }
    }
}
