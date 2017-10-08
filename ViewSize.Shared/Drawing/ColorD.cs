namespace CRLFLabs.ViewSize.Drawing
{
    public struct ColorD
    {
        public ColorD(int red, int green, int blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public int Red { get; }
        public int Green { get; }
        public int Blue { get; }

        public override string ToString()
        {
            return string.Format("[ColorD: Red={0}, Green={1}, Blue={2}]", Red, Green, Blue);
        }
    }
}
