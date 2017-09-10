using System;

namespace CRLFLabs.ViewSize.Drawing
{
    public struct RectangleF
    {
        public RectangleF(double left, double top, double width, double height)
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

        public OriginF Origin => new OriginF(Left, Top);

        public SizeF Size => new SizeF(Width, Height);

        public override string ToString()
            => $"({Left}, {Top}), ({Right}, {Bottom})";

        public RectangleF WithLeft(double left)
            => new RectangleF(left, Top, Width, Height);

        public RectangleF Subtract(RectangleF innerRect)
        {
            // to subtract, we need two points of the two rectangles to be equal
            if (Origin.Equals(innerRect.Origin))
            {
                // top-left are the same
                if (Width == innerRect.Width)
                {
                    // also Width is the same, therefore top-right is the same
                    return new RectangleF(Left, Top + innerRect.Height, Width, Height - innerRect.Height);
                }
                else if (Height == innerRect.Height)
                {
                    // also Height is the same, therefore left-bottom is the same
                    return new RectangleF(Left + innerRect.Width, Top, Width - innerRect.Width, Height);
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

        public RectangleF WithTop(double top)
            => new RectangleF(Left, top, Width, Height);

        public RectangleF WithWidth(double width)
            => new RectangleF(Left, Top, width, Height);

        public RectangleF WithHeight(double height)
            => new RectangleF(Left, Top, Width, height);

        public RectangleF WithOrigin(OriginF origin)
            => new RectangleF(origin.Left, origin.Top, Width, Height);
    }
}
