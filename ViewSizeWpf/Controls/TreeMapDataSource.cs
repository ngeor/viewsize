using System.Collections.Generic;
using CRLFLabs.ViewSize.TreeMap;

namespace ViewSizeWpf.Controls
{
    /// <summary>
    /// A data source for <see cref="TreeMap"/> control.
    /// </summary>
    public class TreeMapDataSource
    {
        /// <summary>
        /// Gets or sets the file system entries with their tree map rectangles.
        /// </summary>
        public IList<FolderWithDrawSize> FoldersWithDrawSize { get; set; }

        /// <summary>
        /// Gets or sets the draw width of the canvas.
        /// </summary>
        public double ActualWidth { get; set; }

        /// <summary>
        /// Gets or sets the draw height of the canvas.
        /// </summary>
        public double ActualHeight { get; set; }
    }
}
