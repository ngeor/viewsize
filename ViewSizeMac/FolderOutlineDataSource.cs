
using AppKit;
using Foundation;
using System.Linq;
using System.Collections.Generic;

namespace ViewSizeMac
{
    public class FolderOutlineDataSource : NSOutlineViewDataSource
    {
        public FolderOutlineDataSource(IList<FSEntryModel> topLevelFolders)
        {
            TopLevelFolders = topLevelFolders;
        }

        private IList<FSEntryModel> TopLevelFolders { get; }

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

        public FSEntryModel Find(string path)
        {
            // TODO optimize this
            foreach (var f in TopLevelFolders)
            {
                var result = f.Find(path);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        private IList<FSEntryModel> ListOf(NSObject item)
        {
            if (item == null)
            {
                return TopLevelFolders;
            }
            else
            {
                return ((FSEntryModel)item).Children;
            }
        }
    }
}
