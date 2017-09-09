using System;
using System.Collections.Generic;
using System.Linq;

namespace CRLFLabs.ViewSize.TreeMap
{
    /// <summary>
    /// Renders a tree map.
    /// </summary>
    class Renderer
    {
        /// <summary>
        /// Delegates the rendering to a function.
        /// This decouples the Renderer from an actual graphics implementation.
        /// </summary>
        /// <value>The do render.</value>
        public DoRender DoRender { get; set; }

        /// <summary>
        /// Renders a tree map of the given file system entries within the given bounds.
        /// </summary>
        /// <param name="bounds">The bounds within to render.</param>
        /// <param name="fileSystemEntries">The file system entries.</param>
        public void Render(RectangleF bounds, IList<IFileSystemEntry> fileSystemEntries)
        {
            Render(bounds, bounds, fileSystemEntries);
        }

        private void Render(RectangleF fullBounds, RectangleF bounds, IList<IFileSystemEntry> fileSystemEntries)
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
            var entries = new LinkedList<IFileSystemEntry>(fileSystemEntries);

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
                        r = r.WithHeight(bounds.Bottom - r.Top);
                    }
                    else
                    {
                        r = r.WithWidth(bounds.Right - r.Left);
                    }
                }

                AssertInBounds(bounds, r);
                DoRender?.Invoke(r);

                // subtree
                Render(fullBounds, r, s.Folder.Children);

                // next
                if (drawVertically)
                {
                    r = r.WithTop(r.Bottom);
                }
                else
                {
                    r = r.WithLeft(r.Right);
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
