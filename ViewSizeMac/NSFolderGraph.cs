﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using AppKit;
using CoreGraphics;
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
        private TreeMapDataSource _dataSource;

        /// <summary>
        /// Holds the bounds of the previously selected area.
        /// Used to repaint more efficiently when selection changes.
        /// </summary>
        private RectangleD? _oldSelectedRect;

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

        public override bool IsOpaque => true;

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

        public override void DrawRect(CGRect dirtyRect)
        {
            DrawHelper drawHelper = new DrawHelper(
                new GraphicsImpl(),
                rect => NeedsToDraw(rect.ToCGRect())
            );

            drawHelper.Draw(DataSource, dirtyRect.ToRectangleD(), DrawScale);
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
            DataSource.PropertyChanging += DataSource_PropertyChanging;
            DataSource.PropertyChanged += DataSource_PropertyChanged;
        }

        private void Detach()
        {
            DataSource.PropertyChanged -= DataSource_PropertyChanged;
            DataSource.PropertyChanging -= DataSource_PropertyChanging;
        }

        void DataSource_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            if (e.PropertyName == TreeMapDataSource.SelectedPropertyName)
            {
                _oldSelectedRect = DataSource.Selected?.Bounds;
            }
        }

        void DataSource_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // gets called when the selection of the treemap data model changes
            // we have the previously selected rect in _oldSelectedRect
            if (e.PropertyName == TreeMapDataSource.SelectedPropertyName)
            {
                SetNeedsDisplayInRect(_oldSelectedRect);
                _oldSelectedRect = null;
                SetNeedsDisplayInRect(DataSource.Selected?.Bounds);
            }
        }

        private void SetNeedsDisplayInRect(RectangleD? bounds)
        {
            if (bounds.HasValue)
            {
                SetNeedsDisplayInRect(bounds.Value.Scale(DrawScale).ToCGRect());
            }
        }
    }
}
