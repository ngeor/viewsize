using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AppKit;
using CoreGraphics;
using CRLFLabs.ViewSize;
using CRLFLabs.ViewSize.Drawing;
using CRLFLabs.ViewSize.IO;
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

        #region Drawing
        /// <summary>
        /// Gets a value indicating whether this <see cref="T:ViewSizeMac.NSFolderGraph"/> is flipped.
        /// A flipped control has the y coordinates originating on the top and increasing towards the bottom.
        /// </summary>
        /// <value><c>true</c> if it is flipped; otherwise, <c>false</c>.</value>
        public override bool IsFlipped => true;

        /// <summary>
        /// Gets the drawing bounds.
        /// </summary>
        /// <value>The drawing bounds.</value>
        public RectangleD BoundsD => Bounds.ToRectangleD();

        /// <summary>
        /// Gets a scale that adjusts datasource coordinates to actual drawing coordinates.
        /// </summary>
        /// <value>The scale to draw.</value>
        private ScaleD DrawScale => DataSource == null ?
            default(ScaleD) : new ScaleD(DataSource.Bounds.Size, BoundsD.Size);

        /// <summary>
        /// Calculates the drawing bounds of the given file system entry.
        /// </summary>
        /// <returns>The drawing bounds of the given entry.</returns>
        /// <param name="entry">The file system entry.</param>
        /// <param name="drawScale">The drawing scale.</param>
        private CGRect CalculateCGRect(FileSystemEntry entry, ScaleD drawScale)
            => entry.Bounds.Scale(drawScale).ToCGRect();


        // TODO improve drawing performance, perhaps with drawing at a bitmap first
        public override void DrawRect(CGRect dirtyRect)
        {
            // clear rect
            NSColor.White.Set();
            NSBezierPath.FillRect(dirtyRect);

            var dataSource = DataSource;
            if (dataSource == null)
            {
                return;
            }

            var drawScale = DrawScale;
            Draw(dataSource.Children, drawScale);
            DrawSelected(drawScale);
        }

        private void DrawSelected(ScaleD drawScale)
        {
            var selected = DataSource?.Selected;
            if (selected != null)
            {
                var rect = CalculateCGRect(selected, drawScale);
                NSColor.White.Set();
                NSBezierPath.StrokeRect(rect);
            }
        }

        private void Draw(IEnumerable<FileSystemEntry> entries, ScaleD drawScale)
        {
            foreach (var entry in entries)
            {
                Draw(entry, drawScale);
            }
        }

        private void Draw(FileSystemEntry entry, ScaleD drawScale)
        {
            if (entry.Children.Any())
            {
                // recursion
                if (NeedsToDraw(CalculateCGRect(entry, drawScale)))
                {
                    Draw(entry.Children, drawScale);
                }
            }
            else
            {
                var rect = CalculateCGRect(entry, drawScale);
                DrawFill(entry, rect);
                DrawOutline(rect);
            }
        }

        private void DrawFill(FileSystemEntry entry, CGRect rect)
        {
            bool isSelected = entry.IsDescendantOf(DataSource.Selected);
            NSColor fillColor = isSelected ? NSColor.Blue : NSColor.Gray;
            NSColor lightColor = isSelected ? NSColor.Cyan : NSColor.LightGray;
            fillColor.Set();
            NSBezierPath.FillRect(rect);

            if (rect.Width >= 5 && rect.Height >= 5)
            {
                NSGradient gradient = new NSGradient(lightColor, fillColor);
                CGPoint middle = new CGPoint(0, 0);
                gradient.DrawInRect(rect, middle);
            }
        }

        private void DrawOutline(CGRect rect)
        {
            NSColor.Black.Set();
            NSBezierPath.StrokeRect(rect);
        }
        #endregion

        public override void MouseUp(NSEvent theEvent)
        {
            base.MouseUp(theEvent);
            var dataSource = DataSource;
            if (dataSource == null)
            {
                return;
            }

            var locationInWindow = theEvent.LocationInWindow;
            var pt = ToClientCoordinates(locationInWindow).ToPointD().Scale(DrawScale.Invert());
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
            var selected = DataSource?.Selected;
            if (selected != null)
            {
                SetNeedsDisplayInRect(CalculateCGRect(selected, DrawScale));
            }
        }
    }
}
