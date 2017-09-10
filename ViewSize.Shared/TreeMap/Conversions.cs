using CRLFLabs.ViewSize.Drawing;
using System;
using System.Collections.Generic;
using System.Text;

namespace CRLFLabs.ViewSize.TreeMap
{
    class Conversions
    {
        public PixelArea PixelSize
        {
            get;
            set;
        }

        public double ByteSize
        {
            get;
            set;
        }

        public PixelArea ToPixelSize(double byteSize) => PixelSize * byteSize / ByteSize;

        public RectangleF FillOneDimension(RectangleF bounds, bool drawVertically, double realSize)
        {
            PixelArea pixelSize = ToPixelSize(realSize);
            return pixelSize.FillOneDimension(bounds, drawVertically);
        }

        public RectangleF FillProportionally(RectangleF bounds, bool drawVertically, double realSize)
        {
            PixelArea pixelSize = ToPixelSize(realSize);
            return pixelSize.FillProportionally(bounds, drawVertically);
        }
    }
}
