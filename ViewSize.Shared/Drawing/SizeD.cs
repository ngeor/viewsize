using System;

namespace CRLFLabs.ViewSize.Drawing
{
    /// <summary>
    /// Represents a size (width and height).
    /// Need to define this as WPF and Cocoa use different structures.
    /// </summary>
    public struct SizeD : IEquatable<SizeD>
    {
        public SizeD(double width, double height)
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

        public bool Equals(SizeD other) => Width == other.Width && Height == other.Height;

        public override string ToString() => $"({Width}, {Height})";

        public static bool operator ==(SizeD left, SizeD right) => left.Equals(right);
        public static bool operator !=(SizeD left, SizeD right) => !left.Equals(right);
    }
}
