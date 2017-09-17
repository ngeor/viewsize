using System.Collections.Generic;
using CRLFLabs.ViewSize.Drawing;

namespace CRLFLabs.ViewSize.TreeMap
{
    public class FolderWithDrawSize
    {
        public FolderWithDrawSize(FolderWithDrawSize parent)
        {
            Parent = parent;
        }

        public IFileSystemEntry Folder { get; set; }
        public RectangleD DrawSize { get; set; }
        public IList<FolderWithDrawSize> Children { get; set; }
        public FolderWithDrawSize Parent { get; }

        /// <summary>
        /// Checks if this object is a descendant of the given object.
        /// </summary>
        public bool IsDescendantOf(FolderWithDrawSize folderWithDrawSize)
        {
            if (folderWithDrawSize == null)
            {
                return false;
            }

            for (var n = this; n != null; n = n.Parent)
            {
                if (n == folderWithDrawSize)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
