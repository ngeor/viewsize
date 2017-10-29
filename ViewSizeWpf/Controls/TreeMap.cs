// <copyright file="TreeMap.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CRLFLabs.ViewSize.Drawing;
using CRLFLabs.ViewSize.Mvp;
using CRLFLabs.ViewSize.TreeMap;
using CRLFLabs.ViewSizeWpf.Common;

namespace ViewSizeWpf.Controls
{
    [Presenter(typeof(TreeMapPresenter))]
    public class TreeMap : FrameworkElement, ITreeMapView
    {
        public TreeMap()
        {
            PresenterFactory.Create(this);
            this.Load?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler Load;

        public IMainModel Model { get; set; }

        public RectangleD BoundsD => new RectangleD(0, 0, this.ActualWidth, this.ActualHeight);

        public event EventHandler RedrawNeeded;

        #region Rendering

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            this.RedrawNeeded?.Invoke(this, EventArgs.Empty);
            var source = this.RenderWithGdi();
            drawingContext.DrawImage(source, new Rect(0, 0, this.ActualWidth, this.ActualHeight));
        }

        private BitmapSource RenderWithGdi()
        {
            int width = (int)this.ActualWidth;
            int height = (int)this.ActualHeight;
            using (var tempBitmap = new Bitmap(width, height))
            {
                using (var g = Graphics.FromImage(tempBitmap))
                {
                    GdiGraphics graphics = new GdiGraphics(g);
                    DrawHelper drawHelper = new DrawHelper(graphics, (r) => true);
                    drawHelper.Draw(this.Model, new RectangleD(0, 0, this.ActualWidth, this.ActualHeight));
                }

                var hbmp = tempBitmap.GetHbitmap();
                var options = BitmapSizeOptions.FromEmptyOptions();
                return Imaging.CreateBitmapSourceFromHBitmap(hbmp, IntPtr.Zero, Int32Rect.Empty, options);
            }
        }

        public void Redraw()
        {
            this.InvalidateVisual();
        }

        public void SelectionChanging()
        {
            // not needed for windows, we just do a full redraw
        }

        public void SelectionChanged()
        {
            // on windows we just do a full redraw
            this.Redraw();
        }

        #endregion
    }
}
