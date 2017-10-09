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

        public void Draw(TreeMapDataSource dataSource, RectangleD dirtyRect, ScaleD drawScale)
        {
            // clear rect
            Graphics.FillRect(Colors.White, dirtyRect);

            if (dataSource == null)
            {
                return;
            }

            var selected = dataSource.Selected;
            DrawChildren(dataSource, drawScale, selected);
            DrawSelected(selected, drawScale);
        }

        private void DrawSelected(FileSystemEntry selected, ScaleD drawScale)
        {
            if (selected != null)
            {
                var rect = selected.Bounds.Scale(drawScale);
                Graphics.DrawRect(Colors.White, rect);
            }
        }

        private void DrawChildren(
            IFileSystemEntryContainer container,
            ScaleD drawScale,
            FileSystemEntry selected)
        {
            foreach (var entry in container.Children)
            {
                Draw(entry, drawScale, selected);
            }
        }

        private void Draw(FileSystemEntry entry, ScaleD drawScale, FileSystemEntry selected)
        {
            var rect = entry.Bounds.Scale(drawScale);
            if (!NeedsToDraw(rect))
            {
                return;
            }

            if (entry.Children.Any())
            {
                // recursion
                DrawChildren(entry, drawScale, selected);
            }
            else
            {
                DrawFill(entry, rect, selected);
            }
        }

        private void DrawFill(FileSystemEntry entry, RectangleD rect, FileSystemEntry selected)
        {
            var fillColor = SelectFillColor(entry);
            var lightColor = fillColor.Lighter();
            var darkColor = fillColor.Darker();
            Graphics.FillRect(fillColor, rect);

            if (rect.Width >= 5 && rect.Height >= 5)
            {
                Graphics.FillEllipseGradient(lightColor, fillColor, rect);
            }

            Graphics.DrawRect(darkColor, rect);
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
