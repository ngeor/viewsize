using System;
using AppKit;
using CoreGraphics;
using CRLFLabs.ViewSize.Drawing;

namespace ViewSizeMac
{
    public class GraphicsImpl : IGraphics
    {
        public void DrawRect(ColorD color, RectangleD rect, int width)
        {
            color.ToNSColor().Set();
            NSGraphics.FrameRect(rect.ToCGRect(), width);
        }

        public void FillEllipseGradient(ColorD inner, ColorD outer, RectangleD rect)
        {
            NSGradient gradient = new NSGradient(inner.ToNSColor(), outer.ToNSColor());
            CGPoint middle = new CGPoint(0, 0);
            gradient.DrawInRect(rect.ToCGRect(), middle);
        }

        public void FillEllipseGradient(ColorD inner, ColorD outer, RectangleD rect, PointD centerPoint)
        {
            NSGradient gradient = new NSGradient(inner.ToNSColor(), outer.ToNSColor());
            CGPoint middle = (rect.Center - centerPoint).ToCGPoint();
            gradient.DrawInRect(rect.ToCGRect(), middle);
        }

        public void FillRect(ColorD color, RectangleD rect)
        {
            color.ToNSColor().Set();
            NSGraphics.RectFill(rect.ToCGRect());
        }
    }
}
