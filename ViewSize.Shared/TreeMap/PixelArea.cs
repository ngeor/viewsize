using CRLFLabs.ViewSize.Drawing;
using System;
using System.Collections.Generic;
using System.Text;

namespace CRLFLabs.ViewSize.TreeMap
{
    /// <summary>
    /// Represents an amount of pixels occupying an area.
    /// </summary>
    struct PixelArea
    {
        /// <summary>
        /// The amount of pixels.
        /// </summary>
        private readonly double amount;

        public PixelArea(double amount)
        {
            this.amount = amount;
        }

        public static PixelArea operator *(PixelArea left, double scale)
        {
            return new PixelArea(left.amount * scale);
        }

        public static PixelArea operator /(PixelArea left, double scale)
        {
            return new PixelArea(left.amount / scale);
        }

        /// <summary>
        /// Fills the given rectangle across one dimension.
        /// If we're drawing vertically, it fills the entire height.
        /// Otherwise, it fills the entire width.
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="drawVertically"></param>
        /// <returns></returns>
        public RectangleF FillOneDimension(RectangleF bounds, bool drawVertically)
        {
            if (drawVertically)
            {
                return bounds.WithWidth(amount / bounds.Height);
            }
            else
            {
                return bounds.WithHeight(amount / bounds.Width);
            }
        }

        /// <summary>
        /// Assuming this area is a sub-area of the given total area, it fills the given bounds proportionally.
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="drawVertically"></param>
        /// <returns></returns>
        public RectangleF FillProportionally(RectangleF bounds, bool drawVertically)
        {
            if (drawVertically)
            {
                return bounds.WithHeight(amount / bounds.Width);
            }
            else
            {
                return bounds.WithWidth(amount / bounds.Height);
            }
        }
    }
}
