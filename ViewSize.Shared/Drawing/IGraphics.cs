namespace CRLFLabs.ViewSize.Drawing
{
    public interface IGraphics
    {
        void FillRect(ColorD color, RectangleD rect);
        void DrawRect(ColorD color, RectangleD rect);
        void FillEllipseGradient(ColorD inner, ColorD outer, RectangleD rect);
    }
}
