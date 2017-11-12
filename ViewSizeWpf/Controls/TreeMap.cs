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
            Load?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler Load;

        public IMainModel Model { get; set; }

        public RectangleD BoundsD => new RectangleD(0, 0, ActualWidth, ActualHeight);

        public event EventHandler RedrawNeeded;

        #region Rendering

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            RedrawNeeded?.Invoke(this, EventArgs.Empty);
            var source = RenderWithGdi();
            drawingContext.DrawImage(source, new Rect(0, 0, ActualWidth, ActualHeight));
        }

        private BitmapSource RenderWithGdi()
        {
            int width = (int)ActualWidth;
            int height = (int)ActualHeight;
            using (var tempBitmap = new Bitmap(width, height))
            {
                using (var g = Graphics.FromImage(tempBitmap))
                {
                    GdiGraphics graphics = new GdiGraphics(g);
                    DrawHelper drawHelper = new DrawHelper(graphics, (r) => true);
                    drawHelper.Draw(Model, new RectangleD(0, 0, ActualWidth, ActualHeight));
                }

                var hbmp = tempBitmap.GetHbitmap();
                var options = BitmapSizeOptions.FromEmptyOptions();
                return Imaging.CreateBitmapSourceFromHBitmap(hbmp, IntPtr.Zero, Int32Rect.Empty, options);
            }
        }

        public void Redraw()
        {
            InvalidateVisual();
        }

        public void SelectionChanging()
        {
            // not needed for windows, we just do a full redraw
        }

        public void SelectionChanged()
        {
            // on windows we just do a full redraw
            Redraw();
        }

        #endregion
    }
}
