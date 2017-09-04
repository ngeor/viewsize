namespace CRLFLabs.ViewSize.TreeMap
{
    public struct RectangleF
    {
        public RectangleF(float left, float top, float width, float height)
        {
            Left = left;
            Top = top;
            Width = width;
            Height = height;
        }

        public float Left { get; set; }
        public float Top { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
    }
}
