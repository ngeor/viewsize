namespace CRLFLabs.ViewSize.Drawing
{
    /// <summary>
    /// A point in 2D space.
    /// </summary>
    public struct PointD
    {
        public PointD(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X { get; }
        public double Y { get; }

        public override string ToString() => $"({X}, {Y})";

        public PointD Scale(ScaleD scale) => new PointD(X * scale.ScaleX, Y * scale.ScaleY);
    }
}
