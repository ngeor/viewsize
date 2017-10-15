using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows.Interop;
using CRLFLabs.ViewSize.Drawing;
using CRLFLabs.ViewSize.TreeMap;
using System.ComponentModel;
using CRLFLabs.ViewSize.Mvp;

namespace ViewSizeWpf.Controls
{
    public class TreeMap : FrameworkElement, ITreeMapView
    {
        private IMainModel _model;

        public IMainModel Model
        {
            get
            {
                return _model;
            }
            set
            {
                Detach(_model);
                _model = value;
                Attach(_model);
                InvalidateVisual();
            }
        }

        public RectangleD BoundsD => new RectangleD(0, 0, ActualWidth, ActualHeight);

        private void Attach(IMainModel model)
        {
            if (model != null)
            {
                model.PropertyChanged += DataSourcePropertyChanged;
            }
        }

        private void Detach(IMainModel model)
        {
            if (model != null)
            {
                model.PropertyChanged -= DataSourcePropertyChanged;
            }
        }

        private void DataSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            InvalidateVisual();
        }

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

        #endregion
    }
}
