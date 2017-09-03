
using AppKit;
using Foundation;
using System.Linq;
using System.Collections.Generic;

namespace ViewSizeMac
{
    public class FolderOutlineDataSource : NSOutlineViewDataSource
    {
        public FolderOutlineDataSource(IList<FolderViewModel> topLevelFolders)
        {
            TopLevelFolders = topLevelFolders;
        }

        private IList<FolderViewModel> TopLevelFolders { get; }

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

        private IList<FolderViewModel> ListOf(NSObject item)
        {
            if (item == null)
            {
                return TopLevelFolders;
            }
            else
            {
                return ((FolderViewModel)item).Children;
            }
        }
    }
}
