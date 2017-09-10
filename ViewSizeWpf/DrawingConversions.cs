using CRLFLabs.ViewSize.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewSizeWpf
{
    /// <summary>
    /// Converts geometric objects from the CRLFLabs shared namespace into the WPF/GDI (and vice versa).
    /// </summary>
    public static class DrawingConversions
    {
        public static RectangleF ToRectangleF(this RectangleD rectangle)
        {
            return new RectangleF((float)rectangle.Left, (float)rectangle.Top, (float)rectangle.Width, (float)rectangle.Height);
        }
    }
}
