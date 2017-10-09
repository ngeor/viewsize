using CRLFLabs.ViewSize.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CRLFLabs.ViewSize.IO
{
    public partial class FileSystemEntry : IFileSystemEntryContainer
    {
        private readonly List<FileSystemEntry> _children = new List<FileSystemEntry>();
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
            AddToChildrenOfParent();
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

        public IReadOnlyList<FileSystemEntry> Children => _children;

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

        internal void AdjustTotalSizeAndSortChildren()
        {
            TotalSize = _children.Select(c => c.TotalSize).Sum();
            _children.Sort((x, y) =>
            {
                if (x.TotalSize > y.TotalSize)
                {
                    return -1;
                }
                else if (x.TotalSize < y.TotalSize)
                {
                    return 1;
                }
                else
                {
                    return x.Path.CompareTo(y.Path);
                }
            });
        }

        private void AddToChildrenOfParent()
        {
            if (_parent is FileSystemEntry parent)
            {
                parent._children.Add(this);
            }
        }
    }
}
