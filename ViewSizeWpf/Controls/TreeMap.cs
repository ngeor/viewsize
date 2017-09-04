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
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            drawingContext.DrawRectangle(Brushes.AliceBlue, new Pen(Brushes.Black, 1), new Rect(0, 0, ActualWidth, ActualHeight));
        }
    }
}
