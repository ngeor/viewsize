using System;
using System.Collections.Generic;
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
}
