using System.Collections.Generic;
using CRLFLabs.ViewSize.Drawing;

namespace CRLFLabs.ViewSize.TreeMap
{
    public class FolderWithDrawSize
    {
        public IFileSystemEntry Folder { get; set; }
        public RectangleD DrawSize { get; set; }
        public IList<FolderWithDrawSize> Children { get; set; }

        /// <summary>
        /// Checks if this object is a descendant of the given object.
        /// </summary>
        public bool IsDescendantOf(FolderWithDrawSize folderWithDrawSize)
        {
            if (folderWithDrawSize == null)
            {
                return false;
            }

            return folderWithDrawSize.ContainsDescendant(this);
        }

        public bool ContainsDescendant(FolderWithDrawSize folderWithDrawSize)
        {
            if (this == folderWithDrawSize)
            {
                return true;
            }

            foreach (var child in Children)
            {
                if (child.ContainsDescendant(folderWithDrawSize))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
