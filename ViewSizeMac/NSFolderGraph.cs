using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AppKit;
using Foundation;

namespace ViewSizeMac
{
    [Register("NSFolderGraph")]
    public class NSFolderGraph : NSControl
    {
        #region Constructors
        public NSFolderGraph()
        {
            Initialize();
        }

        public NSFolderGraph(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        [Export("initWithFrame:")]
        public NSFolderGraph(CoreGraphics.CGRect frameRect) : base(frameRect)
        {
            Initialize();
        }

        private void Initialize()
        {
            WantsLayer = true;
            LayerContentsRedrawPolicy = NSViewLayerContentsRedrawPolicy.OnSetNeedsDisplay;
        }
        #endregion

        private IList<FSEntryModel> dataSource;

        public IList<FSEntryModel> DataSource
        {
            get
            {
                return dataSource;
            }
            set
            {
                dataSource = value;
                NeedsDisplay = true;
            }
        }

        public override void DrawRect(CoreGraphics.CGRect dirtyRect)
        {
            // clear rect
            NSColor.White.Set();
            NSBezierPath.FillRect(dirtyRect);

            if (DataSource == null)
            {
                return;
            }

            DrawColumns(DataSource, Bounds);
        }

        private void DrawColumns(IList<FSEntryModel> fileSystemEntries, CoreGraphics.CGRect bounds)
        {
            // paint left -> right top level folders
            var width = bounds.Width;
            var height = bounds.Height;
            double left = bounds.Left;
            var top = bounds.Top;

            foreach (var item in fileSystemEntries)
            {
                // calculate the bounds where this item will be drawn
                var itemWidth = item.Percentage * width;
                var itemBounds = new CoreGraphics.CGRect(left, top, itemWidth, height);

                if (item.Children.Any())
                {
                    DrawRows(item.Children, itemBounds);
                }
                else
                {
                    NSColor.Red.Set();
                    NSBezierPath.FillRect(itemBounds);
                }

                left = left + itemWidth;
            }
        }

        private void DrawRows(IList<FSEntryModel> fileSystemEntries, CoreGraphics.CGRect bounds)
        {
            // paint top -> bottom folders
            var width = bounds.Width;
            var height = bounds.Height;
            var left = bounds.Left;
            double top = bounds.Top;

            foreach (var item in fileSystemEntries)
            {
                var itemHeight = item.Percentage * height;
                var itemBounds = new CoreGraphics.CGRect(left, top, width, itemHeight);

                if (item.Children.Any())
                {
                    DrawColumns(item.Children, itemBounds);
                }
                else
                {
                    NSColor.Blue.Set();
                    NSBezierPath.FillRect(itemBounds);
                }

                top = top + itemHeight;
            }
        }

        public override bool IsFlipped => true;
    }

    class TreeMap
    {
        public void Render(RectangleF bounds, IList<FSEntryModel> fileSystemEntries)
        {
            // TODO: assumes that fileSystemEntries do not contain zero items
            bool drawVertically = bounds.Width > bounds.Height;

            // e.g. real total size = 200 bytes
            var realTotalSize = fileSystemEntries.Sum(f => f.TotalSize);

            // e.g. draw total size = 100 pixels = 20x5
            var drawTotalSize = bounds.Width * bounds.Height;

            var currentStreak = new List<FSEntryModel>();

            foreach (var entry in fileSystemEntries)
            {
                // e.g. real size of this item = 20 bytes
                var realSize = entry.TotalSize;

                // e.g. draw total size = 10 pixels
                var drawSize = drawTotalSize * realSize / realTotalSize;

                float entryWidth;
                float entryHeight;
                if (drawVertically)
                {
                    entryHeight = bounds.Height;
                    entryWidth = drawSize / entryHeight;
                }
                else
                {
                    entryWidth = bounds.Width;
                    entryHeight = drawSize / entryWidth;
                }

                var aspect = Aspect(entryWidth, entryHeight);

            }

        }

        private static float Aspect(float width, float height) => Math.Max(width / height, height / width);
    }
}
