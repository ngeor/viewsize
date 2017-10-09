using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows.Interop;
using CRLFLabs.ViewSize.Drawing;
using CRLFLabs.ViewSize.TreeMap;
using System.ComponentModel;

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
                Detach(_dataSource);
                _dataSource = value;
                Attach(_dataSource);
                InvalidateVisual();
            }
        }

        private void Attach(TreeMapDataSource dataSource)
        {
            if (dataSource != null)
            {
                dataSource.PropertyChanged += DataSourcePropertyChanged;
            }
        }

        private void Detach(TreeMapDataSource dataSource)
        {
            if (dataSource != null)
            {
                dataSource.PropertyChanged -= DataSourcePropertyChanged;
            }
        }

        private void DataSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            InvalidateVisual();
        }

        #region Rendering

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            var actualBounds = new RectangleD(0, 0, ActualWidth, ActualHeight);
            DataSource?.ReCalculate(actualBounds);
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
                    drawHelper.Draw(DataSource, new RectangleD(0, 0, ActualWidth, ActualHeight));
                }

                var hbmp = tempBitmap.GetHbitmap();
                var options = BitmapSizeOptions.FromEmptyOptions();
                return Imaging.CreateBitmapSourceFromHBitmap(hbmp, IntPtr.Zero, Int32Rect.Empty, options);
            }
        }

        #endregion
    }
}
