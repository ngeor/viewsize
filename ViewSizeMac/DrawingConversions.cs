// <copyright file="DrawingConversions.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;
using AppKit;
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

        public static PointD ToPointD(this CGPoint point)
        {
            return new PointD(point.X, point.Y);
        }

        public static CGPoint ToCGPoint(this PointD point)
        {
            return new CGPoint(point.X, point.Y);
        }

        public static NSColor ToNSColor(this ColorD color)
        {
            return NSColor.FromRgb(color.Red, color.Green, color.Blue);
        }
    }
}
