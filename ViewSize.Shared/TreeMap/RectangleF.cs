using System;

namespace CRLFLabs.ViewSize.TreeMap
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

        public double Left { get; set; }
        public double Top { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Right => Left + Width;
        public double Bottom => Top + Height;

        public override string ToString()
            => $"({Left}, {Top}), ({Right}, {Bottom})";
    }
}
