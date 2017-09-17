using System.Collections.Generic;
using CRLFLabs.ViewSize.Drawing;

namespace CRLFLabs.ViewSize.TreeMap
{
    public class RenderedFileSystemEntry
    {
        public RenderedFileSystemEntry(RenderedFileSystemEntry parent)
        {
            Parent = parent;
        }

        public IFileSystemEntry FileSystemEntry { get; set; }
        public RectangleD Bounds { get; set; }
        public IList<RenderedFileSystemEntry> Children { get; set; }
        public RenderedFileSystemEntry Parent { get; }

        /// <summary>
        /// Checks if this object is a descendant of the given object.
        /// </summary>
        public bool IsDescendantOf(RenderedFileSystemEntry folderWithDrawSize)
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
