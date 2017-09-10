using System;

namespace CRLFLabs.ViewSize.Drawing
{
    /// <summary>
    /// Represents a scaling factor for 2D drawing.
    /// </summary>
    public struct ScaleD
    {
        public ScaleD(double scaleX, double scaleY)
        {
            ScaleX = scaleX;
            ScaleY = scaleY;
        }

        public ScaleD(SizeD sizeFrom, SizeD sizeTo)
        {
            ScaleX = sizeTo.Width / sizeFrom.Width;
            ScaleY = sizeTo.Height / sizeFrom.Height;
        }

        public double ScaleX { get; }
        public double ScaleY { get; }

        public override string ToString() => $"({ScaleX}, {ScaleY})";

        public ScaleD Invert() => new ScaleD(1 / ScaleX, 1 / ScaleY);
    }
}
