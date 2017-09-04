using CRLFLabs.ViewSize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRLFLabs.ViewSize.TreeMap
{
    /// <summary>
    /// Represents a size (width and height).
    /// Need to define this as WPF and Cocoa use different structures.
    /// </summary>
    public struct SizeF
    {
        public SizeF(float width, float height)
        {
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        public float Width { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        public float Height { get; set; }

        /// <summary>
        /// Gets the aspect ratio.
        /// </summary>
        public float AspectRatio => Math.Max(Width / Height, Height / Width);
    }

    public struct Ratio
    {
        public Ratio(float nominator, float denominator)
        {
            Nominator = nominator;
            Denominator = denominator;
        }

        public float Nominator { get; set; }
        public float Denominator { get; set; }
    }

    /// <summary>
    /// Represents an amount of pixels occupying an area.
    /// </summary>
    struct PixelArea
    {
        /// <summary>
        /// The amount of pixels.
        /// </summary>
        private readonly float amount;

        public PixelArea(float amount)
        {
            this.amount = amount;
        }

        public static PixelArea operator *(PixelArea left, Ratio ratio)
        {
            return new PixelArea(left.amount * ratio.Nominator / ratio.Denominator);
        }

        public static PixelArea operator *(PixelArea left, float scale)
        {
            return new PixelArea(left.amount * scale);
        }

        public static PixelArea operator /(PixelArea left, float scale)
        {
            return new PixelArea(left.amount / scale);
        }

        /// <summary>
        /// Fills the given rectangle across one dimension.
        /// If we're drawing vertically, it fills the entire height.
        /// Otherwise, it fills the entire width.
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="drawVertically"></param>
        /// <returns></returns>
        public SizeF FillOneDimension(RectangleF bounds, bool drawVertically)
        {
            if (drawVertically)
            {
                return new SizeF(amount / bounds.Height, bounds.Height);
            }
            else
            {
                return new SizeF(bounds.Width, amount / bounds.Height);
            }
        }

        /// <summary>
        /// Assuming this area is a sub-area of the given total area, it fills the given bounds proportionally.
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="totalArea"></param>
        /// <param name="drawVertically"></param>
        /// <returns></returns>
        public SizeF FillProportionally(SizeF bounds, PixelArea totalArea, bool drawVertically)
        {
            if (drawVertically)
            {
                return new SizeF(bounds.Width, bounds.Height * amount / totalArea.amount);
            }
            else
            {
                return new SizeF(bounds.Width * amount / totalArea.amount, bounds.Height);
            }
        }
    }

    class Conversions
    {
        public PixelArea PixelSize
        {
            get;
            set;
        }

        public float ByteSize
        {
            get;
            set;
        }

        public PixelArea ToPixelSize(float byteSize) => PixelSize * byteSize / ByteSize;

        public SizeF ToSize(RectangleF bounds, bool drawVertically, float realSize)
        {
            PixelArea pixelSize = ToPixelSize(realSize);
            return pixelSize.FillOneDimension(bounds, drawVertically);
        }

        public SizeF ToSize(SizeF bounds, bool drawVertically, float realStreakSize, float realSize)
        {
            PixelArea pixelSize = ToPixelSize(realSize);
            PixelArea streakPixelSize = ToPixelSize(realStreakSize);
            return pixelSize.FillProportionally(bounds, streakPixelSize, drawVertically);
        }
    }

    class TreeMap
    {
        public void Render(RectangleF bounds, IList<Folder> fileSystemEntries)
        {
            Render(bounds, bounds, fileSystemEntries);
        }

        private void Render(RectangleF fullBounds, RectangleF bounds, IList<Folder> fileSystemEntries)
        {
            // TODO: assumes that fileSystemEntries do not contain zero items
            bool drawVertically = bounds.Width > bounds.Height;

            // e.g. real total size = 200 bytes
            var realTotalSize = fileSystemEntries.Sum(f => f.TotalSize);

            // e.g. draw total size = 100 pixels = 20x5
            var drawTotalSize = fullBounds.Width * fullBounds.Height;

            var conversions = new Conversions
            {
                PixelSize = new PixelArea(drawTotalSize),
                ByteSize = realTotalSize
            };

            var streakCandidate = new LinkedList<Folder>();

            float previousAspect = -1;


            // copy entries
            var entries = new LinkedList<Folder>(fileSystemEntries);

            while (entries.Any())
            {
                // remove first entry
                var entry = entries.First();
                entries.RemoveFirst();

                // add to the current streak
                streakCandidate.AddLast(entry);

                // real size of the streak
                var realStreakSize = streakCandidate.Sum(f => f.TotalSize);

                // e.g. draw total size = 10 pixels
                SizeF drawStreakSize = conversions.ToSize(bounds, drawVertically, realStreakSize);

                SizeF[] drawSizes =
                    streakCandidate.Select(f =>
                                           conversions.ToSize(drawStreakSize, drawVertically, realStreakSize, f.TotalSize))
                                   .ToArray();

                var aspects = drawSizes.Select(s => s.AspectRatio);
                var worseAspect = aspects.Max();

                // is the new aspect worse?
                if (previousAspect >= 0 && previousAspect < worseAspect)
                {
                    // it got worse
                    // remove the last item
                    streakCandidate.RemoveLast();

                    // put back in entries
                    entries.AddFirst(entry);

                    // TODO: render streak

                    // TODO: continue in remaining bounds
                    var newList = entries.ToList();
                    entries.Clear();
                    Render(fullBounds, Subtract(bounds, drawStreakSize, drawVertically), newList);
                }
                else
                {
                    // it got better (or we did not have a previous aspect to compare with)
                    // store it for reference
                    previousAspect = worseAspect;
                }
            }
        }

        private RectangleF Subtract(RectangleF bounds, SizeF size, bool drawVertically)
        {
            if (drawVertically)
            {
                return new RectangleF(bounds.Left + size.Width, bounds.Top, bounds.Width - size.Width, bounds.Height);
            }
            else
            {
                return new RectangleF(bounds.Left, bounds.Top + size.Height, bounds.Width, bounds.Height - size.Height);
            }
        }
    }
}
