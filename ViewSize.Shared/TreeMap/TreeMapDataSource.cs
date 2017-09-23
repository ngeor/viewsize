using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CRLFLabs.ViewSize.Drawing;

namespace CRLFLabs.ViewSize.TreeMap
{
    /// <summary>
    /// A data source for a TreeMap kind of control.
    /// </summary>
    public class TreeMapDataSource : INotifyPropertyChanged, IFileSystemEntryContainer
    {
        private FileSystemEntry _selected;
        private IReadOnlyList<FileSystemEntry> _children = new List<FileSystemEntry>();

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the file system entries with their tree map rectangles.
        /// </summary>
        public IReadOnlyList<FileSystemEntry> Children
        {
            get
            {
                return _children;
            }
            set
            {
                if (value == null)
                {
                    throw new NullReferenceException();
                }

                DetachChildren();
                _children = value;
                AttachChildren();
            }
        }

        public IFileSystemEntryContainer Parent
        {
            get
            {
                return null;
            }
            set
            {
                throw new InvalidOperationException();
            }
        }

        public FileSystemEntry Selected
        {
            get
            {
                return _selected;
            }
            set
            {
                _selected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Selected"));
            }
        }

        /// <summary>
        /// Gets or sets the drawing bounds.
        /// This can be used to scale the drawing in the control.
        /// </summary>
        public RectangleD Bounds { get; set; }

        /// <summary>
        /// Finds the file system entry that exists within these coordinates.
        /// </summary>
        /// <returns>The matching file system entry.</returns>
        /// <param name="pt">Point.</param>
        public FileSystemEntry Find(PointD pt)
        {
            // TODO optimize this function
            return Find(pt, Children);
        }

        private FileSystemEntry Find(PointD pt, IEnumerable<FileSystemEntry> folders)
        {
            // find the first folder that contains these coordinates
            var match = folders.FirstOrDefault(f => f.Bounds.Contains(pt));

            // try to find a more specific match in its children, otherwise return the match
            return match == null ? null : (Find(pt, match.Children) ?? match);
        }

        private void DetachChildren()
        {
            foreach (var root in _children)
            {
                root.Parent = null;
            }
        }

        private void AttachChildren()
        {
            foreach (var root in _children)
            {
                root.Parent = this;
            }
        }
    }
}
