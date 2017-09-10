using CRLFLabs.ViewSize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CRLFLabs.ViewSize.TreeMap;
using System.Drawing;
using System.Windows.Interop;
using System.Drawing.Drawing2D;

namespace ViewSizeWpf.Controls
{
    public class TreeMapDataSource
    {
        public IList<FolderWithDrawSize> FoldersWithDrawSize { get; set; }
        public double ActualWidth { get; set; }
        public double ActualHeight { get; set; }
    }

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

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            var source = RenderWithGdi();
            drawingContext.DrawImage(source, new Rect(0, 0, ActualWidth, ActualHeight));
        }

        private static System.Drawing.RectangleF ToRectangleF(CRLFLabs.ViewSize.Drawing.RectangleF rectangle)
        {
            return new System.Drawing.RectangleF((float)rectangle.Left, (float)rectangle.Top, (float)rectangle.Width, (float)rectangle.Height);
        }

        private BitmapSource RenderWithGdi()
        {
            int width = (int)ActualWidth;
            int height = (int)ActualHeight;
            using (var tempBitmap = new Bitmap(width, height))
            {
                using (var g = Graphics.FromImage(tempBitmap))
                {
                    if (DataSource != null)
                    {
                        g.ScaleTransform((float)(ActualWidth / DataSource.ActualWidth), (float)(ActualHeight / DataSource.ActualHeight));
                    }

                    g.FillRectangle(System.Drawing.Brushes.AliceBlue, 0, 0, width, height);

                    var list = DataSource;
                    if (list != null)
                    {
                        foreach (var folderWithSize in list.FoldersWithDrawSize)
                        {
                            var r = folderWithSize.DrawSize;
                            var rf = ToRectangleF(r);
                            g.FillRectangle(
                                System.Drawing.Brushes.AntiqueWhite, rf);

                            GraphicsPath graphicsPath = new GraphicsPath();
                            graphicsPath.AddEllipse(rf);
                            PathGradientBrush brush = new PathGradientBrush(graphicsPath);
                            brush.CenterColor = System.Drawing.Color.White;
                            brush.SurroundColors = new[]
                            {
                                System.Drawing.Color.AntiqueWhite
                            };

                            g.FillEllipse(brush, rf);

                            g.DrawRectangle(Pens.Black, rf.Left, rf.Top, rf.Width, rf.Height);
                        }
                    }
                }

                var hbmp = tempBitmap.GetHbitmap();
                var options = BitmapSizeOptions.FromEmptyOptions();
                return Imaging.CreateBitmapSourceFromHBitmap(hbmp, IntPtr.Zero, Int32Rect.Empty, options);
            }
        }
    }
}
