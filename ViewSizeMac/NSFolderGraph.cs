using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AppKit;
using CoreGraphics;
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
        private FolderWithDrawSize _selected;

        public TreeMapDataSource DataSource
        {
            get
            {
                return _dataSource;
            }
            set
            {
                _dataSource = value;
                _selected = null;
                NeedsDisplay = true;
            }
        }

        public FolderWithDrawSize Selected
        {
            get
            {
                return _selected;
            }
            set
            {
                _selected = value;
                NeedsDisplay = true;
            }
        }


        /// <summary>
        /// Gets a value indicating whether this <see cref="T:ViewSizeMac.NSFolderGraph"/> is flipped.
        /// A flipped control has the y coordinates originating on the top and increasing towards the bottom.
        /// </summary>
        /// <value><c>true</c> if it is flipped; otherwise, <c>false</c>.</value>
        public override bool IsFlipped => true;

        public RectangleD ActualSize => Bounds.ToRectangleD();
        private ScaleD ScaleToActual => DataSource == null ? default(ScaleD) : new ScaleD(DataSource.Bounds.Size, ActualSize.Size);

        public override void DrawRect(CoreGraphics.CGRect dirtyRect)
        {
            // clear rect
            NSColor.White.Set();
            NSBezierPath.FillRect(dirtyRect);

            NSColor.Blue.Set();
            NSBezierPath.FillRect(new CoreGraphics.CGRect(0, 0, 10, 10));

            var dataSource = DataSource;
            if (dataSource == null)
            {
                return;
            }

            var scale = ScaleToActual;
            Draw(dataSource.FoldersWithDrawSize, scale);

            var selected = Selected;
            if (selected != null)
            {
                var rect = selected.DrawSize.Scale(scale).ToCGRect();
                NSColor.White.Set();
                NSBezierPath.StrokeRect(rect);
            }
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

        public override void MouseUp(NSEvent theEvent)
        {
            base.MouseUp(theEvent);
            var dataSource = DataSource;
            if (dataSource == null)
            {
                return;
            }

            var locationInWindow = theEvent.LocationInWindow;
            var pt = ToClientCoordinates(locationInWindow).ToPointD().Scale(ScaleToActual.Invert());
            var folderWithDrawSize = dataSource.Find(pt);
            if (folderWithDrawSize != null)
            {
                MessageBox.ShowMessage(folderWithDrawSize.Folder.Path);
            }
            else
            {
                MessageBox.ShowMessage("no match");
            }
        }

        /// <summary>
        /// Convert coordinates from a mouse event to client coordinates.
        /// </summary>
        /// <returns>The client coordinates.</returns>
        /// <param name="locationInWindow">Location in window.</param>
        private CGPoint ToClientCoordinates(CGPoint locationInWindow)
        {
            // NOTE: this is very strange but it seems to work
            var locationInView = ConvertPointToView(locationInWindow, null);
            var rectInWindow = ConvertRectToView(Bounds, null);
            return new CGPoint(locationInWindow.X - rectInWindow.Left, locationInView.Y);
        }
    }
}
