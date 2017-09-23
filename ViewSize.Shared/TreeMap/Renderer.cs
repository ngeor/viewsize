using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using CRLFLabs.ViewSize.Drawing;

namespace CRLFLabs.ViewSize.TreeMap
{
    /// <summary>
    /// Renders a tree map.
    /// </summary>
    public class Renderer
    {
        private readonly long totalSizeInBytes;
        private readonly double totalSizeInPixels;

        private Renderer(RectangleD fullBounds, IList<FileSystemEntry> fileSystemEntries)
        {
            // e.g. real total size = 200 bytes
            totalSizeInBytes = fileSystemEntries.Sum(f => f.TotalSize);

            // e.g. draw total size = 100 pixels = 20x5
            totalSizeInPixels = fullBounds.Width * fullBounds.Height;
        }

        /// <summary>
        /// Renders a tree map of the given file system entries within the given bounds.
        /// </summary>
        /// <param name="fullBounds">The bounds within to render.</param>
        /// <param name="fileSystemEntries">The file system entries.</param>
        public static TreeMapDataSource Render(RectangleD fullBounds, IList<FileSystemEntry> fileSystemEntries)
        {
            var originalRenderer = new Renderer(fullBounds, fileSystemEntries);
            var partialRenderer = new PartialRenderer(originalRenderer, fullBounds, fileSystemEntries);
            var list = partialRenderer.Render(parent: null);

            var result = new TreeMapDataSource
            {
                Bounds = fullBounds,
                Children = list
            };

            return result;
        }

        public IList<FileSystemEntry> Render(FileSystemEntry parent)
        {
            var partialRenderer = new PartialRenderer(this, parent.Bounds, parent.Children);
            return partialRenderer.Render(parent);
        }

        private double ToPixelSize(double sizeInBytes) => totalSizeInPixels * sizeInBytes / totalSizeInBytes;

        /// <summary>
        /// Fills the given rectangle across one dimension.
        /// If we're drawing vertically, it fills the entire height.
        /// Otherwise, it fills the entire width.
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="drawVertically"></param>
        /// <returns></returns>
        public RectangleD FillOneDimension(RectangleD bounds, bool drawVertically, long sizeInBytes)
        {
            var amount = ToPixelSize(sizeInBytes);
            if (drawVertically)
            {
                return bounds.WithWidth(amount / bounds.Height);
            }
            else
            {
                return bounds.WithHeight(amount / bounds.Width);
            }
        }

        /// <summary>
        /// Assuming this area is a sub-area of the given total area, it fills the given bounds proportionally.
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="drawVertically"></param>
        /// <returns></returns>
        public RectangleD FillProportionally(RectangleD bounds, bool drawVertically, long sizeInBytes)
        {
            var amount = ToPixelSize(sizeInBytes);
            if (drawVertically)
            {
                return bounds.WithHeight(amount / bounds.Width);
            }
            else
            {
                return bounds.WithWidth(amount / bounds.Height);
            }
        }
    }
}