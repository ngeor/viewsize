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
        /// <summary>
        /// Converts this custom rectangle to a GDI rectangle.
        /// </summary>
        /// <param name="rectangle">This custom rectangle.</param>
        /// <returns>The GDI rectangle.</returns>
        public static RectangleF ToRectangleF(this RectangleD rectangle)
        {
            return new RectangleF((float)rectangle.Left, (float)rectangle.Top, (float)rectangle.Width, (float)rectangle.Height);
        }

        /// <summary>
        /// Converts this WPF point to a custom point.
        /// </summary>
        /// <param name="point">This WPF point.</param>
        /// <returns>The custom point.</returns>
        public static PointD ToPointD(this System.Windows.Point point)
        {
            return new PointD(point.X, point.Y);
        }

        /// <summary>
        /// Converts the custom point to system drawing point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static PointF ToPointF(this PointD point)
        {
            return new PointF((float)point.X, (float)point.Y);
        }

        public static Color ToColor(this ColorD color)
        {
            return Color.FromArgb(color.Red, color.Green, color.Blue);
        }
    }
}
