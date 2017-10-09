using System;
using System.IO;
using System.Linq;
using CRLFLabs.ViewSize.Drawing;
using CRLFLabs.ViewSize.IO;

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
            Graphics = graphics;
            NeedsToDraw = needsToDraw;
        }

        public IGraphics Graphics { get; }

        /// <summary>
        /// Gets a function that can determine if a rectangle needs drawing.
        /// </summary>
        private Func<RectangleD, bool> NeedsToDraw { get; }

        public void Draw(TreeMapDataSource dataSource, RectangleD dirtyRect, ScaleD drawScale,
                        bool drawContents = true,
                        bool drawSelected = true)
        {
            if (dataSource == null)
            {
                // clear rect
                Graphics.FillRect(Colors.White, dirtyRect);
                return;
            }

            var selected = dataSource.Selected;

            if (drawContents)
            {
                DrawChildren(dataSource, drawScale, selected, 0);
            }

            if (drawSelected)
            {
                DrawSelected(selected, drawScale);
            }
        }

        private void DrawSelected(FileSystemEntry selected, ScaleD drawScale)
        {
            if (selected != null)
            {
                var rect = selected.Bounds.Scale(drawScale);
                Graphics.DrawRect(Colors.Yellow, rect, 2);
            }
        }

        private void DrawChildren(
            IFileSystemEntryContainer container,
            ScaleD drawScale,
            FileSystemEntry selected,
            int level)
        {
            //if (level > 7)
            //{
            //    return;
            //}
            foreach (var entry in container.Children)
            {
                Draw(entry, drawScale, selected, level + 1);
            }
        }

        private void Draw(FileSystemEntry entry, ScaleD drawScale, FileSystemEntry selected, int level)
        {
            var rect = entry.Bounds.Scale(drawScale);
            if (!NeedsToDraw(rect))
            {
                return;
            }

            if (entry.Children.Any())
            {
                // recursion
                DrawChildren(entry, drawScale, selected, level);
                Graphics.DrawRect(Colors.Black, rect);
            }
            else
            {
                DrawFill(entry, rect, selected, level);
            }
        }

        private void DrawFill(FileSystemEntry entry, RectangleD rect, FileSystemEntry selected, int level)
        {
            var fillColor = SelectFillColor(entry);
            var lightColor = fillColor.Lighter().Lighter();
            Graphics.FillRect(fillColor, rect);

            if (rect.Width >= 5 && rect.Height >= 5)
            {
                var parent = entry.Parent as FileSystemEntry;
                if (parent != null)
                {
                    Graphics.FillEllipseGradient(lightColor, fillColor, rect, parent.Bounds.Center);
                }
                else
                {
                    Graphics.FillEllipseGradient(lightColor, fillColor, rect);
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
