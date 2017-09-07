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
        public SizeF(double width, double height)
        {
            if (width < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width));
            }

            if (height < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height));
            }

            Width = width;
            Height = height;
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// Gets the aspect ratio.
        /// </summary>
        public double AspectRatio => Math.Max(Width / Height, Height / Width);

        public override string ToString() => $"({Width}, {Height})";
    }

    public struct Ratio
    {
        public Ratio(double nominator, double denominator)
        {
            Nominator = nominator;
            Denominator = denominator;
        }

        public double Nominator { get; set; }
        public double Denominator { get; set; }
    }

    /// <summary>
    /// Represents an amount of pixels occupying an area.
    /// </summary>
    struct PixelArea
    {
        /// <summary>
        /// The amount of pixels.
        /// </summary>
        private readonly double amount;

        public PixelArea(double amount)
        {
            this.amount = amount;
        }

        public static PixelArea operator *(PixelArea left, Ratio ratio)
        {
            return new PixelArea(left.amount * ratio.Nominator / ratio.Denominator);
        }

        public static PixelArea operator *(PixelArea left, double scale)
        {
            return new PixelArea(left.amount * scale);
        }

        public static PixelArea operator /(PixelArea left, double scale)
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

        public double ByteSize
        {
            get;
            set;
        }

        public PixelArea ToPixelSize(double byteSize) => PixelSize * byteSize / ByteSize;

        public SizeF ToSize(RectangleF bounds, bool drawVertically, double realSize)
        {
            PixelArea pixelSize = ToPixelSize(realSize);
            return pixelSize.FillOneDimension(bounds, drawVertically);
        }

        public SizeF ToSize(SizeF bounds, bool drawVertically, double realStreakSize, double realSize)
        {
            PixelArea pixelSize = ToPixelSize(realSize);
            PixelArea streakPixelSize = ToPixelSize(realStreakSize);
            return pixelSize.FillProportionally(bounds, streakPixelSize, drawVertically);
        }
    }

    delegate void DoRender(RectangleF bounds);

    class FolderWithDrawSize
    {
        public Folder Folder { get; set; }
        public SizeF DrawSize { get; set; }
    }

    class TreeMap
    {
        public DoRender DoRender { get; set; }
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

            var streakCandidate = new LinkedList<FolderWithDrawSize>();

            double previousAspect = -1;

            // copy entries
            var entries = new LinkedList<Folder>(fileSystemEntries);

            while (entries.Any())
            {
                // remove first entry
                var entry = entries.First();
                entries.RemoveFirst();

                // add to the current streak
                streakCandidate.AddLast(new FolderWithDrawSize
                {
                    Folder = entry
                });

                // real size of the streak
                var realStreakSize = streakCandidate.Sum(f => f.Folder.TotalSize);

                // e.g. draw total size = 10 pixels
                SizeF drawStreakSize = conversions.ToSize(bounds, drawVertically, realStreakSize);

                foreach (var f in streakCandidate)
                {
                    f.DrawSize = conversions.ToSize(drawStreakSize, drawVertically, realStreakSize, f.Folder.TotalSize);
                }

                var aspects = streakCandidate.Select(s => s.DrawSize.AspectRatio);
                var worseAspect = aspects.Max();

                // is the new aspect worse?
                if (previousAspect >= 0 && previousAspect < worseAspect)
                {
                    // it got worse
                    // remove the last item
                    streakCandidate.RemoveLast();

                    // put back in entries
                    entries.AddFirst(entry);

                    // render streak
                    // recalculate streak etc (TODO: this is duplication)
                    realStreakSize = streakCandidate.Sum(f => f.Folder.TotalSize);
                    drawStreakSize = conversions.ToSize(bounds, drawVertically, realStreakSize);
                    foreach (var f in streakCandidate)
                    {
                        f.DrawSize = conversions.ToSize(drawStreakSize, drawVertically, realStreakSize, f.Folder.TotalSize);
                    }

                    DrawStreak(streakCandidate, fullBounds, bounds, drawVertically);

                    // continue in remaining bounds
                    var newList = entries.ToList();
                    entries.Clear();
                    Render(fullBounds, Subtract(bounds, drawStreakSize, drawVertically), newList);
                }
                else
                {
                    // it got better (or we did not have a previous aspect to compare with)
                    // store it for reference
                    previousAspect = worseAspect;

                    // if it's the last item let's draw
                    if (!entries.Any())
                    {
                        DrawStreak(streakCandidate, fullBounds, bounds, drawVertically);
                    }
                }
            }
        }

        private void DrawStreak(LinkedList<FolderWithDrawSize> streakCandidate, RectangleF fullBounds, RectangleF bounds, bool drawVertically)
        {
            var r = bounds;
            foreach (var s in streakCandidate)
            {
                r = Draw(r, s.DrawSize, drawVertically);

                if (s == streakCandidate.Last.Value)
                {
                    // adjust bounds for last item due to rounding errors etc
                    if (drawVertically)
                    {
                        r.Height = bounds.Bottom - r.Top;
                    }
                    else
                    {
                        r.Width = bounds.Right - r.Left;
                    }
                }

                AssertInBounds(bounds, r);
                DoRender?.Invoke(r);

                // subtree
                Render(fullBounds, r, s.Folder.Children);

                // next
                if (drawVertically)
                {
                    r.Top = r.Bottom;
                }
                else
                {
                    r.Left = r.Right;
                }
            }
        }

        private void AssertInBounds(RectangleF outerBounds, RectangleF innerBounds)
        {
            if (innerBounds.Left < outerBounds.Left || innerBounds.Top < outerBounds.Top || innerBounds.Right > outerBounds.Right || innerBounds.Bottom > outerBounds.Bottom)
            {
                throw new InvalidOperationException($"Rectangle {innerBounds} exceeded {outerBounds}");
            }
        }

        private RectangleF Subtract(RectangleF bounds, SizeF size, bool drawVertically)
        {
            if (!drawVertically)
            {
                return new RectangleF(bounds.Left + size.Width, bounds.Top, bounds.Width - size.Width, bounds.Height);
            }
            else
            {
                return new RectangleF(bounds.Left, bounds.Top + size.Height, bounds.Width, bounds.Height - size.Height);
            }
        }

        private RectangleF Draw(RectangleF bounds, SizeF size, bool drawVertically)
        {
            if (drawVertically)
            {
                return new RectangleF(bounds.Left, bounds.Top, bounds.Width, size.Height);
            }
            else
            {
                return new RectangleF(bounds.Left, bounds.Top, size.Width, bounds.Height);
            }
        }
    }
}
