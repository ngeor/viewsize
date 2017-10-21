using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using AppKit;
using CoreGraphics;
using CRLFLabs.ViewSize.Drawing;
using CRLFLabs.ViewSize.IO;
using CRLFLabs.ViewSize.Mvp;
using CRLFLabs.ViewSize.TreeMap;
using Foundation;

namespace ViewSizeMac
{
    /// <summary>
    /// Custom control that renders a tree map graph.
    /// </summary>
    [Register("NSFolderGraph")]
    public class NSFolderGraph : NSControl, ITreeMapView
    {
        /// <summary>
        /// The main model.
        /// </summary>
        private IMainModel _model;

        /// <summary>
        /// Holds the bounds of the previously selected area.
        /// Used to repaint more efficiently when selection changes.
        /// </summary>
        private RectangleD? _oldSelectedRect;

        /// <summary>
        /// Holds the last fully drawn image.
        /// </summary>
        private readonly ImageHolder _lastFullImageHolder = new ImageHolder();

        /// <summary>
        /// Holds the size of the last fully drawn image.
        /// </summary>
        private CGRect _lastFullImageRect;

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

        public IMainModel Model
        {
            get
            {
                return _model;
            }
            set
            {
                if (_model != null)
                {
                    Detach();
                }

                _model = value;

                if (_model != null)
                {
                    Attach();
                }

                Redraw();
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
        /// Gets a value indicating whether this <see cref="T:ViewSizeMac.NSFolderGraph"/> is opaque.
        /// </summary>
        /// <remarks>
        /// Returns <c>true</c> for performance reasons.
        /// </remarks>
        /// <value><c>true</c> if is opaque; otherwise, <c>false</c>.</value>
        public override bool IsOpaque => true;

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:ViewSizeMac.NSFolderGraph"/> wants default clipping.
        /// </summary>
        /// <remarks>
        /// Returns <c>false</c> for performance reasons.
        /// </remarks>
        /// <value><c>true</c> if wants default clipping; otherwise, <c>false</c>.</value>
        public override bool WantsDefaultClipping => false;

        /// <summary>
        /// Gets the drawing bounds.
        /// </summary>
        /// <value>The drawing bounds.</value>
        public RectangleD BoundsD => Bounds.ToRectangleD();

        private NSBitmapImageRep DrawInBitmap(Action action)
        {
            NSBitmapImageRep bitmapImageRep = new NSBitmapImageRep(
                IntPtr.Zero,
                (int)Bounds.Width,
                (int)Bounds.Height,
                8,
                4,
                true,
                false,
                NSColorSpace.CalibratedRGB,
                0,
                0
            );

            NSGraphicsContext context = NSGraphicsContext.FromBitmap(bitmapImageRep);
            NSGraphicsContext.GlobalSaveGraphicsState();
            NSGraphicsContext.CurrentContext = context;

            action();
            NSGraphicsContext.GlobalRestoreGraphicsState();
            context.Dispose();
            return bitmapImageRep;
        }

        private void DrawRectDuringLiveResize(CGRect dirtyRect)
        {
            if (_lastFullImageHolder.HasValue)
            {
                // we have the last image, scale it even if it becomes fuzzy
                _lastFullImageHolder.Image.Draw(
                    dirtyRect,
                    _lastFullImageRect,
                    NSCompositingOperation.Copy,
                    1,
                    false,
                    null);
            }
            else
            {
                // no last image, just paint everything white
                NSColor.White.Set();
                NSGraphics.RectFill(dirtyRect);
            }
        }

        private void DrawRectNotResizing(CGRect dirtyRect)
        {
            DrawHelper drawHelper = new DrawHelper(
                new GraphicsImpl(),
                rect => NeedsToDraw(rect.ToCGRect())
            );

            // if bounds haven't changed since we drew the last full image
            if (_lastFullImageRect == Bounds && _lastFullImageHolder.HasValue)
            {
                // draw the previous image
                _lastFullImageHolder.Image.Draw(
                    dirtyRect,
                    dirtyRect,
                    NSCompositingOperation.Copy,
                    1,
                    false,
                    null);

                // draw the selection on top
                drawHelper.Draw(Model, dirtyRect.ToRectangleD(), drawContents: false);
            }
            else
            {
                var bitmapImageRep = DrawInBitmap(() =>
                {
                    drawHelper.Draw(Model, dirtyRect.ToRectangleD(), drawSelected: false);
                });
                NSImage image = new NSImage(Bounds.Size);
                image.AddRepresentation(bitmapImageRep);
                image.Draw(dirtyRect,
                           dirtyRect,
                           NSCompositingOperation.Copy,
                           1,
                           false, null);
                
                drawHelper.Draw(Model, dirtyRect.ToRectangleD(), drawContents: false);

                bool isFullDraw = dirtyRect == Bounds;

                if (isFullDraw)
                {
                    // store this image for resizing
                    _lastFullImageHolder.Image = image;
                    _lastFullImageRect = dirtyRect;
                }
                else
                {
                    image.Dispose();
                }
            }
        }

        /// <summary>
        /// Draws the view.
        /// </summary>
        /// <param name="dirtyRect">The rectangle that needs drawing.</param>
        public override void DrawRect(CGRect dirtyRect)
        {
            if (InLiveResize)
            {
                // being resized, don't draw too much
                DrawRectDuringLiveResize(dirtyRect);
            }
            else
            {
                // not doing a live resize, regular drawing
                DrawRectNotResizing(dirtyRect);
            }
        }

        public override void ViewWillStartLiveResize()
        {
            base.ViewWillStartLiveResize();
        }

        public override void ViewDidEndLiveResize()
        {
            base.ViewDidEndLiveResize();

            RedrawNeeded?.Invoke(this, EventArgs.Empty);

            // request full redraw
            NeedsDisplay = true;
        }

        #endregion

        public override void MouseUp(NSEvent theEvent)
        {
            base.MouseUp(theEvent);
            var dataSource = Model;
            if (dataSource == null || dataSource.Children == null)
            {
                return;
            }

            var locationInWindow = theEvent.LocationInWindow;
            var pt = ToClientCoordinates(locationInWindow).ToPointD();
            var folderWithDrawSize = dataSource.Children.Find(pt);
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
            Model.PropertyChanging += Model_PropertyChanging;
            Model.PropertyChanged += Model_PropertyChanged;
        }

        private void Detach()
        {
            Model.PropertyChanged -= Model_PropertyChanged;
            Model.PropertyChanging -= Model_PropertyChanging;
        }

        void Model_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            if (e.PropertyName == MainModel.SelectedPropertyName)
            {
                _oldSelectedRect = Model.Selected?.Bounds;
            }
        }

        void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // gets called when the selection of the treemap data model changes
            // we have the previously selected rect in _oldSelectedRect
            switch (e.PropertyName)
            {
                case MainModel.SelectedPropertyName:
                    SetNeedsDisplayInRect(_oldSelectedRect);
                    _oldSelectedRect = null;
                    SetNeedsDisplayInRect(Model.Selected?.Bounds);
                    break;
                case MainModel.ChildrenPropertyName:
                    Redraw();
                    break;
            }
        }

        private void SetNeedsDisplayInRect(RectangleD? bounds)
        {
            if (bounds.HasValue)
            {
                SetNeedsDisplayInRect(bounds.Value.ToCGRect());
            }
        }

        public event EventHandler RedrawNeeded;

        public void Redraw()
        {
            _lastFullImageHolder.Dispose();
            _oldSelectedRect = null;
            NeedsDisplay = true;
        }
    }
}
