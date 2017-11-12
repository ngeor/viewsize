// <copyright file="RectangleD.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;

namespace CRLFLabs.ViewSize.Drawing
{
    public struct RectangleD
    {
        public RectangleD(double left, double top, double width, double height)
        {
            if (left < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(left));
            }

            if (top < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(top));
            }

            if (width < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width));
            }

            if (height < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height));
            }

            Left = left;
            Top = top;
            Width = width;
            Height = height;
        }

        public double Left { get; }

        public double Top { get; }

        public double Width { get; }

        public double Height { get; }

        public double Right => Left + Width;

        public double Bottom => Top + Height;

        public OriginD Origin => new OriginD(Left, Top);

        public SizeD Size => new SizeD(Width, Height);

        public PointD Center => new PointD(Left + Width / 2, Top + Height / 2);

        public bool IsEmpty => Width < 1 || Height < 1;

        public override string ToString()
            => $"({Left}, {Top}), ({Right}, {Bottom})";

        public RectangleD Subtract(RectangleD innerRect)
        {
            // to subtract, we need two points of the two rectangles to be equal
            if (Origin.Equals(innerRect.Origin))
            {
                // top-left are the same
                if (Width == innerRect.Width)
                {
                    // also Width is the same, therefore top-right is the same
                    return new RectangleD(Left, Top + innerRect.Height, Width, Height - innerRect.Height);
                }
                else if (Height == innerRect.Height)
                {
                    // also Height is the same, therefore left-bottom is the same
                    return new RectangleD(Left + innerRect.Width, Top, Width - innerRect.Width, Height);
                }
                else
                {
                    throw new InvalidOperationException("Subtract cannot return a rectangle");
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public bool Contains(PointD point)
        {
            return Left <= point.X && point.X < Right && Top <= point.Y && point.Y < Bottom;
        }

        public RectangleD WithWidth(double width)
            => new RectangleD(Left, Top, width, Height);

        public RectangleD WithHeight(double height)
            => new RectangleD(Left, Top, Width, height);
    }
}
