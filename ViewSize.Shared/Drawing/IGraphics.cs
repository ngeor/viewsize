namespace CRLFLabs.ViewSize.Drawing
{
    public interface IGraphics
    {
        void DrawRect(ColorD color, RectangleD rect, int width = 1);
        void FillRect(ColorD color, RectangleD rect);
        void FillEllipseGradient(ColorD inner, ColorD outer, RectangleD rect);
    }
}
