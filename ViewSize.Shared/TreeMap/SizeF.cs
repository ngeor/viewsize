using CRLFLabs.ViewSize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRLFLabs.ViewSize.TreeMap
{
    /// <summary>
    /// Represents a size (width and height).
    /// Need to define this as WPF and Cocoa use different structures.
    /// </summary>
    public struct SizeF
    {
        public SizeF(double width, double height)
        {
            if (width < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width));
            }

            if (height < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height));
            }

            Width = width;
            Height = height;
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        public double Width { get; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        public double Height { get; }

        /// <summary>
        /// Gets the aspect ratio.
        /// </summary>
        public double AspectRatio => Math.Max(Width / Height, Height / Width);

        public override string ToString() => $"({Width}, {Height})";
    }

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
                return bounds.WithHeight(amount / bounds.Height);
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

    class Conversions
    {
        public PixelArea PixelSize
        {
            get;
            set;
        }

        public double ByteSize
        {
            get;
            set;
        }

        public PixelArea ToPixelSize(double byteSize) => PixelSize * byteSize / ByteSize;

        public RectangleF FillOneDimension(RectangleF bounds, bool drawVertically, double realSize)
        {
            PixelArea pixelSize = ToPixelSize(realSize);
            return pixelSize.FillOneDimension(bounds, drawVertically);
        }

        public RectangleF FillProportionally(RectangleF bounds, bool drawVertically, double realSize)
        {
            PixelArea pixelSize = ToPixelSize(realSize);
            return pixelSize.FillProportionally(bounds, drawVertically);
        }
    }

    delegate void DoRender(RectangleF bounds);

    class FolderWithDrawSize
    {
        public IFileSystemEntry Folder { get; set; }
        public RectangleF DrawSize { get; set; }
    }
}
