
using AppKit;

namespace ViewSizeMac
{
    class FolderOutlineDataSource : NSOutlineViewDataSource
    {
        public override System.nint GetChildrenCount(NSOutlineView outlineView, Foundation.NSObject item)
        {
            return 0;
        }
    }
}
