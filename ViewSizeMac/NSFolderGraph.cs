using System;
using System.Collections.Generic;
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

        private List<FolderViewModel> dataSource;

        public List<FolderViewModel> DataSource
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
            NSColor.Yellow.Set();
            NSBezierPath.FillRect(dirtyRect);

            if (DataSource == null)
            {
                return;
            }

            DrawColumns(DataSource, Bounds);
        }

        private void DrawColumns(IList<FolderViewModel> folders, CoreGraphics.CGRect bounds)
        {
            // paint left -> right top level folders
            var width = bounds.Width;
            var height = bounds.Height;
            double left = bounds.Left;
            var top = bounds.Top;

            foreach (var item in folders)
            {
                var itemWidth = item.Percentage * width;
                var itemBounds = new CoreGraphics.CGRect(left + 1, top + 1, itemWidth - 2, height - 2);
                NSColor.Red.Set();
                NSBezierPath.FillRect(itemBounds);

                DrawRows(item.Children, itemBounds);

                left = left + itemWidth;
            }
        }

        private void DrawRows(IList<FolderViewModel> folders, CoreGraphics.CGRect bounds)
        {
            // paint top -> bottom folders
            var width = bounds.Width;
            var height = bounds.Height;
            var left = bounds.Left;
            double top = bounds.Top;

            foreach (var item in folders)
            {
                var itemHeight = item.Percentage * height;
                var itemBounds = new CoreGraphics.CGRect(left + 1, top + 1, width - 2, itemHeight - 2);
                NSColor.Blue.Set();
                NSBezierPath.FillRect(itemBounds);
                top = top + itemHeight;
            }
        }

        public override bool IsFlipped => true;
    }
}
