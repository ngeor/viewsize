﻿// <copyright file="IGraphics.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

namespace CRLFLabs.ViewSize.Drawing
{
    public interface IGraphics
    {
        void DrawRect(ColorD color, RectangleD rect, int width = 1);

        void FillRect(ColorD color, RectangleD rect);

        void FillEllipseGradient(ColorD inner, ColorD outer, RectangleD rect);

        void FillEllipseGradient(ColorD inner, ColorD outer, RectangleD rect, PointD centerPoint);
    }
}
