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

        private SizeD Scale =>
            new SizeD(ActualWidth / DataSource.ActualWidth, ActualHeight / DataSource.ActualHeight);

        private void Render(Graphics g)
        {
            var list = DataSource;
            if (list == null)
            {
                g.FillRectangle(System.Drawing.Brushes.AliceBlue, 0, 0, (float)ActualWidth, (float)ActualHeight);
                return;
            }

            SizeD scale = Scale;

            g.FillRectangle(System.Drawing.Brushes.AliceBlue, 0, 0, (float)DataSource.ActualWidth, (float)DataSource.ActualHeight);
            foreach (var folderWithSize in list.FoldersWithDrawSize)
            {
                Render(g, folderWithSize, scale);
            }
        }

        private static void Render(Graphics g, FolderWithDrawSize folderWithSize, SizeD scale)
        {
            var r = folderWithSize.DrawSize.Scale(scale);
            var rf = r.ToRectangleF();
            g.FillRectangle(
                System.Drawing.Brushes.AntiqueWhite, rf);

            GraphicsPath graphicsPath = new GraphicsPath();
            graphicsPath.AddEllipse(rf);
            PathGradientBrush brush = new PathGradientBrush(graphicsPath)
            {
                CenterColor = System.Drawing.Color.White,
                SurroundColors = new[]
                {
                    System.Drawing.Color.AntiqueWhite
                }
            };

            g.FillEllipse(brush, rf);

            g.DrawRectangle(Pens.Black, rf.Left, rf.Top, rf.Width, rf.Height);
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
            PointD point = new PointD(pt.X * DataSource.ActualWidth / ActualWidth, pt.Y * DataSource.ActualHeight / ActualHeight);
            return FolderAtPoint(point, list.FoldersWithDrawSize);
        }

        private FolderWithDrawSize FolderAtPoint(PointD point, IList<FolderWithDrawSize> foldersWithDrawSize)
        {
            // TODO: need to have a hierarchy similar to Folder here, in order to apply recursion
            return foldersWithDrawSize.FirstOrDefault(f => f.DrawSize.Contains(point));
        }
    }
}
