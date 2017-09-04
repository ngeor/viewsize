using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AppKit;
using Foundation;

namespace ViewSizeMac
{
    [Register("NSFolderGraph")]
    public class NSFolderGraph : NSControl
    {
        #region Constructors
        public NSFolderGraph()
        {
            Initialize();
        }

        public NSFolderGraph(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        [Export("initWithFrame:")]
        public NSFolderGraph(CoreGraphics.CGRect frameRect) : base(frameRect)
        {
            Initialize();
        }

        private void Initialize()
        {
            WantsLayer = true;
            LayerContentsRedrawPolicy = NSViewLayerContentsRedrawPolicy.OnSetNeedsDisplay;
        }
        #endregion

        private IList<FSEntryModel> dataSource;

        public IList<FSEntryModel> DataSource
        {
            get
            {
                return dataSource;
            }
            set
            {
                dataSource = value;
                NeedsDisplay = true;
            }
        }

        public override void DrawRect(CoreGraphics.CGRect dirtyRect)
        {
            // clear rect
            NSColor.White.Set();
            NSBezierPath.FillRect(dirtyRect);

            if (DataSource == null)
            {
                return;
            }

            DrawColumns(DataSource, Bounds);
        }

        private void DrawColumns(IList<FSEntryModel> fileSystemEntries, CoreGraphics.CGRect bounds)
        {
            // paint left -> right top level folders
            var width = bounds.Width;
            var height = bounds.Height;
            double left = bounds.Left;
            var top = bounds.Top;

            foreach (var item in fileSystemEntries)
            {
                // calculate the bounds where this item will be drawn
                var itemWidth = item.Percentage * width;
                var itemBounds = new CoreGraphics.CGRect(left, top, itemWidth, height);

                if (item.Children.Any())
                {
                    DrawRows(item.Children, itemBounds);
                }
                else
                {
                    NSColor.Red.Set();
                    NSBezierPath.FillRect(itemBounds);
                }

                left = left + itemWidth;
            }
        }

        private void DrawRows(IList<FSEntryModel> fileSystemEntries, CoreGraphics.CGRect bounds)
        {
            // paint top -> bottom folders
            var width = bounds.Width;
            var height = bounds.Height;
            var left = bounds.Left;
            double top = bounds.Top;

            foreach (var item in fileSystemEntries)
            {
                var itemHeight = item.Percentage * height;
                var itemBounds = new CoreGraphics.CGRect(left, top, width, itemHeight);

                if (item.Children.Any())
                {
                    DrawColumns(item.Children, itemBounds);
                }
                else
                {
                    NSColor.Blue.Set();
                    NSBezierPath.FillRect(itemBounds);
                }

                top = top + itemHeight;
            }
        }

        public override bool IsFlipped => true;
    }

    struct PixelArea
    {
        private readonly float area;

        public PixelArea(float area)
        {
            this.area = area;
        }

        public PixelArea Scale(float part, float total)
        {
            return new PixelArea(part * area / total);
        }

        public SizeF ToSize(RectangleF bounds, bool drawVertically)
        {
            if (drawVertically)
            {
                return new SizeF(area / bounds.Height, bounds.Height);
            }
            else
            {
                return new SizeF(bounds.Width, area / bounds.Height);
            }
        }

        public SizeF ToSize(SizeF bounds, PixelArea totalArea, bool drawVertically)
        {
            if (drawVertically)
            {
                return new SizeF(bounds.Width, bounds.Height * area / totalArea.area);
            }
            else
            {
                return new SizeF(bounds.Width * area / totalArea.area, bounds.Height);
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

        public PixelArea ToPixelSize(float byteSize) => PixelSize.Scale(byteSize, ByteSize);

        public SizeF ToSize(RectangleF bounds, bool drawVertically, float realSize)
        {
            PixelArea pixelSize = ToPixelSize(realSize);
            return pixelSize.ToSize(bounds, drawVertically);
        }

        public SizeF ToSize(SizeF bounds, bool drawVertically, float realStreakSize, float realSize)
        {
            PixelArea pixelSize = ToPixelSize(realSize);
            PixelArea streakPixelSize = ToPixelSize(realStreakSize);
            return pixelSize.ToSize(bounds, streakPixelSize, drawVertically);
        }
    }

    class TreeMap
    {
        public void Render(RectangleF bounds, IList<FSEntryModel> fileSystemEntries)
        {
            Render(bounds, bounds, fileSystemEntries);
        }

        private void Render(RectangleF fullBounds, RectangleF bounds, IList<FSEntryModel> fileSystemEntries)
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

            var streakCandidate = new LinkedList<FSEntryModel>();

            float previousAspect = -1;


            // copy entries
            var entries = new LinkedList<FSEntryModel>(fileSystemEntries);

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

                var aspects = drawSizes.Select(s => s.Aspect());
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

    static class AspectExtensions
    {
        public static float Aspect(this SizeF size)
        {
            return Math.Max(size.Width / size.Height, size.Height / size.Width);
        }
    }
}
