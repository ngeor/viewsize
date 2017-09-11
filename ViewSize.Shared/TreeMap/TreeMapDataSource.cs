using System;
using System.Collections.Generic;
using CRLFLabs.ViewSize.Drawing;

namespace CRLFLabs.ViewSize.TreeMap
{
    /// <summary>
    /// A data source for a TreeMap kind of control.
    /// </summary>
    public class TreeMapDataSource
    {
        /// <summary>
        /// Gets or sets the file system entries with their tree map rectangles.
        /// </summary>
        public IList<FolderWithDrawSize> FoldersWithDrawSize { get; set; }

        /// <summary>
        /// Gets or sets the drawing bounds.
        /// This can be used to scale the drawing in the control.
        /// </summary>
        public RectangleD Bounds { get; set; }
    }
}
