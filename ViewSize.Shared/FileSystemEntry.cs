using CRLFLabs.ViewSize.Drawing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CRLFLabs.ViewSize
{
    public interface IFileSystemEntryContainer
    {
        IList<FileSystemEntry> Children { get; }
        IFileSystemEntryContainer Parent { get; }
    }

    public partial class FileSystemEntry : IFileSystemEntryContainer
    {
        // core properties
        public string Path { get; set; }
        public long OwnSize { get; set; }

        // computed
        public virtual long TotalSize { get; set; }
        public double Percentage { get; set; }

        // relationships
        public virtual IFileSystemEntryContainer Parent { get; set; }
        public virtual IList<FileSystemEntry> Children { get; set; }

        // UI
        public string DisplayText { get; set; }
        public string DisplaySize { get; set; }

        // TreeMap
        public RectangleD Bounds { get; set; }

        /// <summary>
        /// Checks if this object is a descendant of the given object.
        /// </summary>
        public bool IsDescendantOf(FileSystemEntry otherFolder)
        {
            if (otherFolder == null)
            {
                return false;
            }

            for (IFileSystemEntryContainer n = this; n != null; n = n.Parent)
            {
                if (n == otherFolder)
                {
                    return true;
                }
            }

            return false;
        }

        public IEnumerable<FileSystemEntry> AncestorsNearestFirst()
        {
            FileSystemEntry parent = Parent as FileSystemEntry;
            if (parent == null)
            {
                return Enumerable.Empty<FileSystemEntry>();
            }
            else
            {
                return Enumerable.Repeat(parent, 1).Concat(parent.AncestorsNearestFirst());
            }
        }
    }

    public static class FileSystemEntryContainerExtensions
    { 
        public static IFileSystemEntryContainer RootContainer(this IFileSystemEntryContainer container)
        {
            if (container.Parent == null)
            {
                return container;
            }

            return container.Parent.RootContainer();
        }
    }
}
