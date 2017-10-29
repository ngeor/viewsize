// <copyright file="ColorD.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;

namespace CRLFLabs.ViewSize.Drawing
{
    public struct ColorD
    {
        public ColorD(int red, int green, int blue)
        {
            this.Red = red;
            this.Green = green;
            this.Blue = blue;
        }

        public int Red { get; }

        public int Green { get; }

        public int Blue { get; }

        public override string ToString()
        {
            return string.Format("[ColorD: Red={0}, Green={1}, Blue={2}]", this.Red, this.Green, this.Blue);
        }

        public ColorD Lighter() => this.Adjust(1.5f);

        public ColorD Darker() => this.Adjust(0.66f);

        public ColorD Adjust(float factor)
        {
            return new ColorD(Cap(this.Red * factor), Cap(this.Green * factor), Cap(this.Blue * factor));
        }

        private static int Cap(float value)
        {
            return (int)Math.Min(255, value);
        }
    }
}
