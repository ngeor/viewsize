using CRLFLabs.ViewSize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ViewSizeWpf.Controls
{
    public class TreeMap : FrameworkElement
    {
        private static readonly Pen BlackPen = new Pen(Brushes.Black, 1);
        private IList<IFileSystemEntry> _dataSource;

        public IList<IFileSystemEntry> DataSource
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

            drawingContext.DrawRectangle(Brushes.AliceBlue, BlackPen, new Rect(0, 0, ActualWidth, ActualHeight));

            var list = DataSource;
            if (list == null)
            {
                return;
            }
                        
            var treeMap = new CRLFLabs.ViewSize.TreeMap.Renderer
            {
                DoRender = (r) => DoRender(drawingContext, r)
            };

            var bounds = new CRLFLabs.ViewSize.TreeMap.RectangleF(0, 0, ActualWidth, ActualHeight);
            treeMap.Render(bounds, DataSource);
        }

        private void DoRender(DrawingContext drawingContext, CRLFLabs.ViewSize.TreeMap.RectangleF rectangle)
        {
            drawingContext.DrawRectangle(Brushes.AntiqueWhite, BlackPen, ToRect(rectangle));
        }

        private static Rect ToRect(CRLFLabs.ViewSize.TreeMap.RectangleF rectangle)
        {
            return new Rect(rectangle.Left, rectangle.Top, rectangle.Width, rectangle.Height);
        }
    }
}
