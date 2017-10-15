using CRLFLabs.ViewSize.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CRLFLabs.ViewSize.IO
{
    public partial class FileSystemEntry
    {
        private readonly List<FileSystemEntry> _children = new List<FileSystemEntry>();

        public FileSystemEntry(string path, FileSystemEntry parent)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            Path = path;
            Parent = parent;
            AddToChildrenOfParent();
        }

        // core properties
        public string Path { get; }
        public long OwnSize { get; set; }
        public bool IsDirectory { get; set; }

        // computed
        // virtual for unit tests
        public virtual long TotalSize { get; private set; }
        public double Percentage { get; set; }
        public long FileCount { get; private set; }

        // relationships
        public FileSystemEntry Parent { get; }

        // virtual for unit tests
        public virtual IReadOnlyList<FileSystemEntry> Children => _children;

        public bool IsTopLevel => Parent == null;

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

            for (var n = this; n != null; n = n.Parent)
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
            if (!IsDirectory)
            {
                TotalSize = OwnSize;
                FileCount = 1;
                return;
            }

            TotalSize = _children.Select(c => c.TotalSize).Sum();
            FileCount = _children.Select(c => c.FileCount).Sum();
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
            Parent?._children.Add(this);
        }
    }
}
