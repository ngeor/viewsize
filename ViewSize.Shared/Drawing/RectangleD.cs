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

            this.Left = left;
            this.Top = top;
            this.Width = width;
            this.Height = height;
        }

        public double Left { get; }

        public double Top { get; }

        public double Width { get; }

        public double Height { get; }

        public double Right => this.Left + this.Width;

        public double Bottom => this.Top + this.Height;

        public OriginD Origin => new OriginD(this.Left, this.Top);

        public SizeD Size => new SizeD(this.Width, this.Height);

        public PointD Center => new PointD(this.Left + this.Width / 2, this.Top + this.Height / 2);

        public bool IsEmpty => this.Width < 1 || this.Height < 1;

        public override string ToString()
            => $"({this.Left}, {this.Top}), ({this.Right}, {this.Bottom})";

        public RectangleD Subtract(RectangleD innerRect)
        {
            // to subtract, we need two points of the two rectangles to be equal
            if (this.Origin.Equals(innerRect.Origin))
            {
                // top-left are the same
                if (this.Width == innerRect.Width)
                {
                    // also Width is the same, therefore top-right is the same
                    return new RectangleD(this.Left, this.Top + innerRect.Height, this.Width, this.Height - innerRect.Height);
                }
                else if (this.Height == innerRect.Height)
                {
                    // also Height is the same, therefore left-bottom is the same
                    return new RectangleD(this.Left + innerRect.Width, this.Top, this.Width - innerRect.Width, this.Height);
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
            return this.Left <= point.X && point.X < this.Right && this.Top <= point.Y && point.Y < this.Bottom;
        }

        public RectangleD WithWidth(double width)
            => new RectangleD(this.Left, this.Top, width, this.Height);

        public RectangleD WithHeight(double height)
            => new RectangleD(this.Left, this.Top, this.Width, height);
    }
}
