using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CRLFLabs.ViewSize.Drawing;
using CRLFLabs.ViewSize.IO;

namespace CRLFLabs.ViewSize.TreeMap
{
    /// <summary>
    /// A data source for a TreeMap kind of control.
    /// </summary>
    public class TreeMapDataSource : INotifyPropertyChanged, INotifyPropertyChanging, IFileSystemEntryContainer
    {
        public const string SelectedPropertyName = "Selected";

        private FileSystemEntry _selected;

        public TreeMapDataSource(IEnumerable<FileSystemEntry> topLevelEntries, RectangleD bounds)
        {
            Children = topLevelEntries.Select(SetParent).ToList();
            Bounds = bounds;
        }

        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the file system entries with their tree map rectangles.
        /// </summary>
        public IReadOnlyList<FileSystemEntry> Children { get; }

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
                if (_selected != value)
                {
                    PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(SelectedPropertyName));
                    _selected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(SelectedPropertyName));
                }
            }
        }

        private RectangleD Bounds { get; set; }

        public void ReCalculate(RectangleD bounds)
        {
            if (Bounds.Size == bounds.Size)
            {
                return;
            }

            Renderer.Render(bounds, Children);
            Bounds = bounds;
        }

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

        private FileSystemEntry SetParent(FileSystemEntry entry)
        {
            entry.Parent = this;
            return entry;
        }
    }
}
