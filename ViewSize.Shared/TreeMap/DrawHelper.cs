// <copyright file="DrawHelper.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CRLFLabs.ViewSize.Drawing;
using CRLFLabs.ViewSize.IO;
using CRLFLabs.ViewSize.Mvp;

namespace CRLFLabs.ViewSize.TreeMap
{
    /// <summary>
    /// Cross-platform helper for drawing a treemap datasource.
    /// </summary>
    public class DrawHelper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:CRLFLabs.ViewSize.TreeMap.DrawHelper"/> class.
        /// </summary>
        /// <param name="graphics">The graphics system to use.</param>
        /// <param name="needsToDraw">A function that can determin if a rectangle needs drawing.</param>
        public DrawHelper(IGraphics graphics, Func<RectangleD, bool> needsToDraw)
        {
            this.Graphics = graphics;
            this.NeedsToDraw = needsToDraw;
        }

        public IGraphics Graphics { get; }

        /// <summary>
        /// Gets a function that can determine if a rectangle needs drawing.
        /// </summary>
        private Func<RectangleD, bool> NeedsToDraw { get; }

        public void Draw(IMainModel model, RectangleD dirtyRect,
                        bool drawContents = true,
                        bool drawSelected = true)
        {
            if (model == null || model.Children == null)
            {
                // clear rect
                this.Graphics.FillRect(Colors.White, dirtyRect);
                return;
            }

            var selected = model.Selected;

            if (drawContents)
            {
                this.DrawChildren(model.Children, selected, 0);
            }

            if (drawSelected)
            {
                this.DrawSelected(selected);
            }
        }

        private void DrawSelected(FileSystemEntry selected)
        {
            if (selected != null)
            {
                var rect = selected.Bounds;
                this.Graphics.DrawRect(Colors.Yellow, rect, 2);
            }
        }

        private void DrawChildren(
            IReadOnlyList<FileSystemEntry> elements,
            FileSystemEntry selected,
            int level)
        {
            // if (level > 7)
            // {
            //    return;
            // }
            foreach (var entry in elements)
            {
                this.Draw(entry, selected, level + 1);
            }
        }

        private void Draw(FileSystemEntry entry, FileSystemEntry selected, int level)
        {
            var rect = entry.Bounds;
            if (!this.NeedsToDraw(rect))
            {
                return;
            }

            if (entry.Children.Any())
            {
                // recursion
                this.DrawChildren(entry.Children, selected, level);
                this.Graphics.DrawRect(Colors.Black, rect);
            }
            else
            {
                this.DrawFill(entry, rect, selected, level);
            }
        }

        private void DrawFill(FileSystemEntry entry, RectangleD rect, FileSystemEntry selected, int level)
        {
            var fillColor = this.SelectFillColor(entry);
            var lightColor = fillColor.Lighter().Lighter();
            this.Graphics.FillRect(fillColor, rect);

            if (rect.Width >= 5 && rect.Height >= 5)
            {
                var parent = entry.Parent as FileSystemEntry;
                if (parent != null)
                {
                    this.Graphics.FillEllipseGradient(lightColor, fillColor, rect, parent.Bounds.Center);
                }
                else
                {
                    this.Graphics.FillEllipseGradient(lightColor, fillColor, rect);
                }
            }
        }

        private ColorD SelectFillColor(FileSystemEntry entry)
        {
            switch (Path.GetExtension(entry.Path))
            {
                case ".jpg":
                case ".png":
                case ".gif":
                case ".bmp":
                    return Colors.Red;
                case ".zip":
                    return Colors.Green;
                case ".xml":
                    return Colors.Yellow;
                case ".avi":
                case ".mp4":
                    return Colors.Purple;
                case ".mp3":
                    return Colors.Pink;
                case ".pdf":
                case ".docx":
                case ".xlsx":
                    return Colors.Blue;
                default:
                    return Colors.Gray;
            }
        }
    }
}
