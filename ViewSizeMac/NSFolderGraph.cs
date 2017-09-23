using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AppKit;
using CoreGraphics;
using CRLFLabs.ViewSize;
using CRLFLabs.ViewSize.Drawing;
using CRLFLabs.ViewSize.TreeMap;
using Foundation;

namespace ViewSizeMac
{
    /// <summary>
    /// Custom control that renders a tree map graph.
    /// </summary>
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
                if (_dataSource != null)
                {
                    Detach();
                }

                _dataSource = value;

                if (_dataSource != null)
                {
                    Attach();
                }

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

        public override void DrawRect(CGRect dirtyRect)
        {
            // clear rect
            NSColor.White.Set();
            NSBezierPath.FillRect(dirtyRect);

            NSColor.Blue.Set();
            NSBezierPath.FillRect(new CGRect(0, 0, 10, 10));

            var dataSource = DataSource;
            if (dataSource == null)
            {
                return;
            }

            var scale = ScaleToActual;
            Draw(dataSource.Children, scale);

            var selected = dataSource.Selected;
            if (selected != null)
            {
                var rect = selected.Bounds.Scale(scale).ToCGRect();
                NSColor.White.Set();
                NSBezierPath.StrokeRect(rect);
            }
        }

        private void Draw(IEnumerable<FileSystemEntry> renderedFileSystemEntries, ScaleD scaleToActual)
        {
            foreach (var renderedFileSystemEntry in renderedFileSystemEntries)
            {
                Draw(renderedFileSystemEntry, scaleToActual);
            }
        }

        private void Draw(FileSystemEntry renderedFileSystemEntry, ScaleD scaleToActual)
        {
            var rect = renderedFileSystemEntry.Bounds.Scale(scaleToActual).ToCGRect();
            if (renderedFileSystemEntry.IsDescendantOf(DataSource.Selected))
            {
                NSColor.Brown.Set();
            }
            else
            {
                NSColor.Red.Set();
            }

            NSBezierPath.FillRect(rect);
            NSColor.Black.Set();
            NSBezierPath.StrokeRect(rect);

            // recursion
            Draw(renderedFileSystemEntry.Children, scaleToActual);
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
            dataSource.Selected = folderWithDrawSize;
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

        private void Attach()
        {
            DataSource.PropertyChanged += DataSource_PropertyChanged;
        }

        private void Detach()
        {
            DataSource.PropertyChanged -= DataSource_PropertyChanged;
        }

        void DataSource_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // gets called when the selection of the treemap data model changes
            NeedsDisplay = true;
        }
    }
}
