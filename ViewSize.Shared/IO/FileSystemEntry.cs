using CRLFLabs.ViewSize.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CRLFLabs.ViewSize.IO
{
    public partial class FileSystemEntry : IFileSystemEntryContainer
    {
        private IFileSystemEntryContainer _parent;

        public FileSystemEntry(string path, IFileSystemEntryContainer parent)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (parent == null)
            {
                throw new ArgumentNullException(nameof(parent));
            }

            Path = path;
            _parent = parent;
        }

        // core properties
        public string Path { get; }
        public long OwnSize { get; set; }
        public bool IsDirectory { get; set; }

        // computed
        public long TotalSize { get; set; }
        public double Percentage { get; set; }

        // relationships
        public IFileSystemEntryContainer Parent
        {
            get => _parent;

            internal set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                if (!IsTopLevel)
                {
                    throw new InvalidOperationException("Can only reparent top-level entries");
                }

                if (value is FileSystemEntry)
                {
                    throw new InvalidOperationException("New parent cannot be FileSystemEntry");
                }

                _parent = value;
            }
        }

        // TODO delete setter
        public IReadOnlyList<FileSystemEntry> Children { get; set; }

        public bool IsTopLevel => !(Parent is FileSystemEntry);

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
            var parent = Parent as FileSystemEntry;
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
            var parent = Parent as FileSystemEntry;
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
}
