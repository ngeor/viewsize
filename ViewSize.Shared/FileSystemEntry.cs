using CRLFLabs.ViewSize.Drawing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CRLFLabs.ViewSize
{
    public interface IFileSystemEntryContainer
    {
        IReadOnlyList<FileSystemEntry> Children { get; }
        IFileSystemEntryContainer Parent { get; }
    }

    public partial class FileSystemEntry : IFileSystemEntryContainer
    {
        public FileSystemEntry(string path)
        {
            Path = path;    
        }

        // core properties
        public string Path { get; }
        public long OwnSize { get; set; }

        // computed
        public long TotalSize { get; set; }
        public double Percentage { get; set; }

        // relationships
        public IFileSystemEntryContainer Parent { get; set; }
        public IReadOnlyList<FileSystemEntry> Children { get; set; }

        // UI
        public string DisplayText { get; set; }
        public string DisplaySize { get; set; }

        // TreeMap
        public RectangleD Bounds { get; set; }

        public override string ToString()
        {
            return $"[FileSystemEntry: Path={Path}]";
        }

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

        // TODO: is it possible to use Ancestors in all cases?
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

        public IEnumerable<FileSystemEntry> Ancestors()
        {
            FileSystemEntry parent = Parent as FileSystemEntry;
            if (parent == null)
            {
                return Enumerable.Empty<FileSystemEntry>();
            }
            else
            {
                return parent.Ancestors().Concat(Enumerable.Repeat(parent, 1));
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
