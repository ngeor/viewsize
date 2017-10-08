using System;
using System.IO;
using System.Linq;
using CRLFLabs.ViewSize.Drawing;
using CRLFLabs.ViewSize.IO;

namespace CRLFLabs.ViewSize.TreeMap
{
    public class DrawHelper
    {
        public DrawHelper(IGraphics graphics, Func<RectangleD, bool> needsToDraw)
        {
            Graphics = graphics;
            NeedsToDraw = needsToDraw;
        }

        public IGraphics Graphics { get; }
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

        private void DrawChildren(IFileSystemEntryContainer container, ScaleD drawScale, FileSystemEntry selected)
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
                DrawOutline(rect);
            }
        }

        private void DrawFill(FileSystemEntry entry, RectangleD rect, FileSystemEntry selected)
        {
            bool isSelected = entry.IsDescendantOf(selected);
            var fillColor = isSelected ? Colors.Blue : Colors.Gray;
            var lightColor = isSelected ? Colors.LightBlue : Colors.LightGray;
            if (Path.GetExtension(entry.Path) == ".jpg")
            {
                fillColor = Colors.Red;
                lightColor = Colors.LightRed;
            }

            Graphics.FillRect(fillColor, rect);

            if (rect.Width >= 5 && rect.Height >= 5)
            {
                Graphics.FillEllipseGradient(lightColor, fillColor, rect);
            }
        }

        private void DrawOutline(RectangleD rect)
        {
            Graphics.DrawRect(Colors.Black, rect);
        }
    }
}
