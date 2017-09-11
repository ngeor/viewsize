using System;
using CoreGraphics;
using CRLFLabs.ViewSize.Drawing;

namespace ViewSizeMac
{
    /// <summary>
    /// Converts between the common custom structures in ViewSize.Shared and CoreGraphics.
    /// </summary>
    public static class DrawingConversions
    {
        public static RectangleD ToRectangleD(this CGRect rect)
        {
            return new RectangleD(rect.Left, rect.Top, rect.Width, rect.Height);
        }

        public static CGRect ToCGRect(this RectangleD rect)
        {
            return new CGRect(rect.Left, rect.Top, rect.Width, rect.Height);
        }
    }
}
