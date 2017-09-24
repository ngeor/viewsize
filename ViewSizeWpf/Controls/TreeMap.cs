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
using System.ComponentModel;
using CRLFLabs.ViewSize;

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
        public ScaleD ScaleToActual => new ScaleD(DataSource.Bounds.Size, ActualSize);

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
            Render(g, dataSource.Children, scale);

            var selected = dataSource.Selected;
            if (selected != null)
            {
                var rect = selected.Bounds.Scale(scale).ToRectangleF();
                g.DrawRectangle(Pens.White, rect.Left, rect.Top, rect.Width, rect.Height);
            }
        }

        private void Render(Graphics g, IEnumerable<FileSystemEntry> folders, ScaleD scale)
        {
            foreach (var folderWithSize in folders)
            {
                Render(g, folderWithSize, scale);
            }
        }

        private void Render(Graphics g, FileSystemEntry folderWithSize, ScaleD scale)
        {
            // scale rectangle to actual drawing dimensions and convert to GDI
            var rect = folderWithSize.Bounds.Scale(scale).ToRectangleF();
            bool isUnderSelected = folderWithSize.IsDescendantOf(DataSource?.Selected);
            var brush = isUnderSelected ?
                System.Drawing.Brushes.Blue :
                System.Drawing.Brushes.Gray;

            g.FillRectangle(brush, rect);

            // draw ellipse gradient
            // the following condition is for performance optimization
            // TODO: use common rendering code/abstraction for wpf/cocoa
            // TODO: why certain font folders appear to be larger than their contents?
            if (rect.Width >= 5 && rect.Height >= 5 && !folderWithSize.Children.Any())
            {
                using (GraphicsPath graphicsPath = new GraphicsPath())
                {
                    graphicsPath.AddEllipse(rect);
                    using (PathGradientBrush gradientBrush = new PathGradientBrush(graphicsPath)
                    {
                        CenterColor = isUnderSelected ? System.Drawing.Color.Cyan : System.Drawing.Color.LightGray,
                        SurroundColors = new[]
                        {
                        isUnderSelected ? System.Drawing.Color.Blue :  System.Drawing.Color.Gray
                    }
                    })
                    {
                        g.FillEllipse(gradientBrush, rect);
                    }
                }
            }

            // draw outline
            g.DrawRectangle(Pens.Black, rect.Left, rect.Top, rect.Width, rect.Height);

            Render(g, folderWithSize.Children, scale);
        }
        #endregion
    }
}
