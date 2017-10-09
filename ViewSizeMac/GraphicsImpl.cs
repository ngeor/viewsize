﻿using AppKit;
using CoreGraphics;
using CRLFLabs.ViewSize.Drawing;

namespace ViewSizeMac
{
    public class GraphicsImpl : IGraphics
    {
        public void DrawRect(ColorD color, RectangleD rect)
        {
            color.ToNSColor().Set();
            NSGraphics.FrameRect(rect.ToCGRect());
        }

        public void FillEllipseGradient(ColorD inner, ColorD outer, RectangleD rect)
        {
            NSGradient gradient = new NSGradient(inner.ToNSColor(), outer.ToNSColor());
            CGPoint middle = new CGPoint(0, 0);
            gradient.DrawInRect(rect.ToCGRect(), middle);
        }

        public void FillRect(ColorD color, RectangleD rect)
        {
            color.ToNSColor().Set();
            NSGraphics.RectFill(rect.ToCGRect());
        }
    }
}
