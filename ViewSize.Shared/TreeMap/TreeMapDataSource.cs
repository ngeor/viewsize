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
    public class TreeMapDataSource : INotifyPropertyChanged
    {
        private FolderWithDrawSize _selected;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the file system entries with their tree map rectangles.
        /// </summary>
        public IList<FolderWithDrawSize> FoldersWithDrawSize { get; set; }

        public FolderWithDrawSize Selected
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
        public FolderWithDrawSize Find(PointD pt)
        {
            return Find(pt, FoldersWithDrawSize);
        }

        private FolderWithDrawSize Find(PointD pt, IEnumerable<FolderWithDrawSize> folders)
        {
            // find the first folder that contains these coordinates
            var match = folders.FirstOrDefault(f => f.DrawSize.Contains(pt));

            // try to find a more specific match in its children, otherwise return the match
            return match == null ? null : (Find(pt, match.Children) ?? match);
        }

        /// <summary>
        /// Finds the folder entry of the given path.
        /// </summary>
        /// <returns>The matching file system entry.</returns>
        /// <param name="path">The path to find.</param>
        public FolderWithDrawSize Find(string path)
        {
            // TODO optimize this
            return Find(path, FoldersWithDrawSize);
        }

        private FolderWithDrawSize Find(string path, IEnumerable<FolderWithDrawSize> folders)
        {
            // TODO optimize this
            foreach (var f in folders)
            {
                if (f.Folder.Path == path)
                {
                    return f;
                }

                var childResult = Find(path, f.Children);
                if (childResult != null)
                {
                    return childResult;
                }
            }

            return null;
        }
    }
}
