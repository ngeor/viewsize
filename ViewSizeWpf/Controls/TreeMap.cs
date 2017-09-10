using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows.Interop;
using System.Drawing.Drawing2D;
using CRLFLabs.ViewSize.Drawing;
using CRLFLabs.ViewSize.TreeMap;
using System.Collections.Generic;
using System.Linq;

namespace ViewSizeWpf.Controls
{
    public class TreeMap : FrameworkElement
    {
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
                InvalidateVisual();
            }
        }

        #region Rendering
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
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
                    Render(g);
                }

                var hbmp = tempBitmap.GetHbitmap();
                var options = BitmapSizeOptions.FromEmptyOptions();
                return Imaging.CreateBitmapSourceFromHBitmap(hbmp, IntPtr.Zero, Int32Rect.Empty, options);
            }
        }

        /// <summary>
        /// Gets the actual size of this control.
        /// </summary>
        public SizeD ActualSize => new SizeD(ActualWidth, ActualHeight);

        /// <summary>
        /// Gets the scaling factor in order to convert datasource sizes into actual sizes.
        /// </summary>
        private ScaleD ScaleToActual => new ScaleD(DataSource.DrawSize, ActualSize);

        private void Render(Graphics g)
        {
            // clear background
            g.FillRectangle(System.Drawing.Brushes.AliceBlue, 0, 0, (float)ActualWidth, (float)ActualHeight);

            var dataSource = DataSource;
            if (dataSource == null)
            {
                return;
            }

            ScaleD scale = ScaleToActual;
            foreach (var folderWithSize in dataSource.FoldersWithDrawSize)
            {
                Render(g, folderWithSize, scale);
            }
        }

        private static void Render(Graphics g, FolderWithDrawSize folderWithSize, ScaleD scale)
        {
            // scale rectangle to actual drawing dimensions and convert to GDI
            var rect = folderWithSize.DrawSize.Scale(scale).ToRectangleF();
            g.FillRectangle(
                System.Drawing.Brushes.AntiqueWhite, rect);

            // draw ellipse gradient
            GraphicsPath graphicsPath = new GraphicsPath();
            graphicsPath.AddEllipse(rect);
            PathGradientBrush brush = new PathGradientBrush(graphicsPath)
            {
                CenterColor = System.Drawing.Color.White,
                SurroundColors = new[]
                {
                    System.Drawing.Color.AntiqueWhite
                }
            };

            g.FillEllipse(brush, rect);

            // draw outline
            g.DrawRectangle(Pens.Black, rect.Left, rect.Top, rect.Width, rect.Height);
        }
        #endregion

        public FolderWithDrawSize FolderAtPoint(System.Windows.Point pt)
        {
            var list = DataSource;
            if (list == null)
            {
                return null;
            }

            // scale back the point into the datasource coordinates
            PointD point = pt.ToPointD().Scale(ScaleToActual.Invert());
            return FolderAtPoint(point, list.FoldersWithDrawSize);
        }

        private FolderWithDrawSize FolderAtPoint(PointD point, IList<FolderWithDrawSize> foldersWithDrawSize)
        {
            // TODO: need to have a hierarchy similar to Folder here, in order to apply recursion
            return foldersWithDrawSize.FirstOrDefault(f => f.DrawSize.Contains(point));
        }
    }
}
