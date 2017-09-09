using CRLFLabs.ViewSize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ViewSizeWpf.Controls
{
    public class TreeMap : FrameworkElement
    {
        private IList<FileSystemEntry> _dataSource;

        public IList<FileSystemEntry> DataSource
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

            drawingContext.DrawRectangle(Brushes.AliceBlue, new Pen(Brushes.Black, 1), new Rect(0, 0, ActualWidth, ActualHeight));

            var list = DataSource;
            if (list == null)
            {
                return;
            }

            var treeMap = new CRLFLabs.ViewSize.TreeMap.TreeMap
            {
                DoRender = (r) => DoRender(drawingContext, r)
            };
            var bounds = new CRLFLabs.ViewSize.TreeMap.RectangleF(0, 0, ActualWidth, ActualHeight);
            treeMap.Render(bounds, DataSource);
        }

        private void DoRender(DrawingContext drawingContext, CRLFLabs.ViewSize.TreeMap.RectangleF rectangle)
        {
            drawingContext.DrawRectangle(Brushes.AntiqueWhite, new Pen(Brushes.Black, 1), ToRect(rectangle));
        }

        private static Rect ToRect(CRLFLabs.ViewSize.TreeMap.RectangleF rectangle)
        {
            return new Rect(rectangle.Left, rectangle.Top, rectangle.Width, rectangle.Height);
        }
    }
}
