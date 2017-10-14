﻿using System;
using System.Collections.Generic;
using System.Linq;
using CRLFLabs.ViewSize.Drawing;
using CRLFLabs.ViewSize.IO;

namespace CRLFLabs.ViewSize.TreeMap
{
    /// <summary>
    /// Renders a tree map.
    /// </summary>
    public class Renderer
    {
        private readonly long totalSizeInBytes;
        private readonly double totalSizeInPixels;
        private readonly RectangleD _fullBounds;
        private readonly IReadOnlyList<FileSystemEntry> _fileSystemEntries;

        public Renderer(RectangleD fullBounds, IReadOnlyList<FileSystemEntry> fileSystemEntries)
        {
            // e.g. real total size = 200 bytes
            totalSizeInBytes = fileSystemEntries.Sum(Measurer);

            // e.g. draw total size = 100 pixels = 20x5
            totalSizeInPixels = fullBounds.Width * fullBounds.Height;

            _fullBounds = fullBounds;
            _fileSystemEntries = fileSystemEntries;
        }

        public void Render()
        {
            var partialRenderer = new PartialRenderer(this, _fullBounds, _fileSystemEntries);
            partialRenderer.Render();
        }

        internal void Render(FileSystemEntry parent)
        {
            var partialRenderer = new PartialRenderer(this, parent.Bounds, parent.Children);
            partialRenderer.Render();
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
        public void FillProportionally(RectangleD bounds, bool drawVertically, LinkedList<FileSystemEntry> streakCandidate)
        {
            double lastLeft = bounds.Left;
            double lastTop = bounds.Top;
            foreach (var entry in streakCandidate)
            {
                var amount = ToPixelSize(Measurer(entry));
                if (drawVertically)
                {
                    entry.Bounds = new RectangleD(bounds.Left, lastTop, bounds.Width, amount / bounds.Width);
                    lastTop = entry.Bounds.Bottom;
                }
                else
                {
                    entry.Bounds = new RectangleD(lastLeft, bounds.Top, amount / bounds.Height, bounds.Height);
                    lastLeft = entry.Bounds.Right;
                }
            }
        }

        public Func<FileSystemEntry, long> Measurer { get; set; } = TotalSizeMeasurer;

        public static Func<FileSystemEntry, long> TotalSizeMeasurer = f => f.TotalSize;
        public static Func<FileSystemEntry, long> FileCountMeasurer = f => f.FileCount;
    }
}