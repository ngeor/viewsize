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
        public double Width { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// Gets the aspect ratio.
        /// </summary>
        public double AspectRatio => Math.Max(Width / Height, Height / Width);

        public override string ToString() => $"({Width}, {Height})";
    }

    public struct Ratio
    {
        public Ratio(double nominator, double denominator)
        {
            Nominator = nominator;
            Denominator = denominator;
        }

        public double Nominator { get; set; }
        public double Denominator { get; set; }
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

        public static PixelArea operator *(PixelArea left, Ratio ratio)
        {
            return new PixelArea(left.amount * ratio.Nominator / ratio.Denominator);
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
        public SizeF FillOneDimension(RectangleF bounds, bool drawVertically)
        {
            if (drawVertically)
            {
                return new SizeF(amount / bounds.Height, bounds.Height);
            }
            else
            {
                return new SizeF(bounds.Width, amount / bounds.Height);
            }
        }

        /// <summary>
        /// Assuming this area is a sub-area of the given total area, it fills the given bounds proportionally.
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="totalArea"></param>
        /// <param name="drawVertically"></param>
        /// <returns></returns>
        public SizeF FillProportionally(SizeF bounds, PixelArea totalArea, bool drawVertically)
        {
            if (drawVertically)
            {
                return new SizeF(bounds.Width, bounds.Height * amount / totalArea.amount);
            }
            else
            {
                return new SizeF(bounds.Width * amount / totalArea.amount, bounds.Height);
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

        public SizeF ToSize(RectangleF bounds, bool drawVertically, double realSize)
        {
            PixelArea pixelSize = ToPixelSize(realSize);
            return pixelSize.FillOneDimension(bounds, drawVertically);
        }

        public SizeF ToSize(SizeF bounds, bool drawVertically, double realStreakSize, double realSize)
        {
            PixelArea pixelSize = ToPixelSize(realSize);
            PixelArea streakPixelSize = ToPixelSize(realStreakSize);
            return pixelSize.FillProportionally(bounds, streakPixelSize, drawVertically);
        }
    }

    delegate void DoRender(RectangleF bounds);

    class FolderWithDrawSize
    {
        public IFileSystemEntry Folder { get; set; }
        public SizeF DrawSize { get; set; }
    }
}
