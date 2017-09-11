using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AppKit;
using CRLFLabs.ViewSize.Drawing;
using CRLFLabs.ViewSize.TreeMap;
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
            LayerContentsRedrawPolicy = NSViewLayerContentsRedrawPolicy.DuringViewResize;
        }
        #endregion

        private TreeMapDataSource _dataSource;

        public TreeMapDataSource DataSource
        {
            get
            {
                return _dataSource;
            }
            set
            {
                _dataSource = value;
                NeedsDisplay = true;
            }
        }

        public RectangleD ActualSize => Bounds.ToRectangleD();
        private ScaleD ScaleToActual => DataSource == null ? default(ScaleD) : new ScaleD(DataSource.Bounds.Size, ActualSize.Size);

        public override void DrawRect(CoreGraphics.CGRect dirtyRect)
        {
            // clear rect
            NSColor.White.Set();
            NSBezierPath.FillRect(dirtyRect);

            var dataSource = DataSource;
            if (dataSource == null)
            {
                return;
            }

            var scale = ScaleToActual;
            Draw(dataSource.FoldersWithDrawSize, scale);
        }

        private void Draw(IEnumerable<FolderWithDrawSize> foldersWithDrawSize, ScaleD scaleToActual)
        {
            foreach (var folderWithDrawSize in foldersWithDrawSize)
            {
                Draw(folderWithDrawSize, scaleToActual);
            }
        }

        private void Draw(FolderWithDrawSize folderWithDrawSize, ScaleD scaleToActual)
        {
            var rect = folderWithDrawSize.DrawSize.Scale(scaleToActual).ToCGRect();
            NSColor.Red.Set();
            NSBezierPath.FillRect(rect);
            NSColor.Black.Set();
            NSBezierPath.StrokeRect(rect);

            // recursion
            Draw(folderWithDrawSize.Children, scaleToActual);
        }

        public override bool IsFlipped => true;
    }
}
