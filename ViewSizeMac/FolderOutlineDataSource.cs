
using AppKit;
using Foundation;
using System.Linq;

namespace ViewSizeMac
{
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
