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
    public class TreeMapDataSource<T> : INotifyPropertyChanged
        where T : class, IFileSystemEntry<T>
    {
        private T _selected;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the file system entries with their tree map rectangles.
        /// </summary>
        public IList<T> FoldersWithDrawSize { get; set; }

        public T Selected
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
        public T Find(PointD pt)
        {
            return Find(pt, FoldersWithDrawSize);
        }

        private T Find(PointD pt, IEnumerable<T> folders)
        {
            // find the first folder that contains these coordinates
            var match = folders.FirstOrDefault(f => f.Bounds.Contains(pt));

            // try to find a more specific match in its children, otherwise return the match
            return match == null ? null : (Find(pt, match.Children) ?? match);
        }

        /// <summary>
        /// Finds the folder entry of the given path.
        /// </summary>
        /// <returns>The matching file system entry.</returns>
        /// <param name="path">The path to find.</param>
        public T Find(string path)
        {
            // TODO optimize this
            return FoldersWithDrawSize.Find(path);
        }
    }
}
