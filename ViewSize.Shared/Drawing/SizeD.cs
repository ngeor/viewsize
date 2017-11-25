// <copyright file="SizeD.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

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
        /// Gets the width.
        /// </summary>
        public double Width { get; }

        /// <summary>
        /// Gets the height.
        /// </summary>
        public double Height { get; }

        /// <summary>
        /// Gets the aspect ratio.
        /// </summary>
        public double AspectRatio => Math.Max(Width / Height, Height / Width);

        public static bool operator ==(SizeD left, SizeD right) => left.Equals(right);

        public static bool operator !=(SizeD left, SizeD right) => !left.Equals(right);

        public bool Equals(SizeD other) => Width == other.Width && Height == other.Height;

        public override bool Equals(object obj)
        {
            return obj is SizeD size && this.Equals(size);
        }

        public override int GetHashCode()
        {
            return Width.GetHashCode() ^ Height.GetHashCode();
        }

        public override string ToString() => $"({Width}, {Height})";
    }
}
