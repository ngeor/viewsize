using CRLFLabs.ViewSize.Drawing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CRLFLabs.ViewSize.TreeMap
{
    class PartialRenderer
    {
        private readonly Renderer renderer;
        private readonly bool drawVertically;
        private readonly IList<IFileSystemEntry> fileSystemEntries;
        private readonly RectangleD bounds;

        public PartialRenderer(Renderer renderer, RectangleD bounds, IList<IFileSystemEntry> fileSystemEntries)
        {
            this.renderer = renderer;
            this.fileSystemEntries = fileSystemEntries;
            this.bounds = bounds;
            drawVertically = bounds.Width > bounds.Height;
        }

        public IList<RenderedFileSystemEntry> Render(RenderedFileSystemEntry parent)
        {
            var streakCandidate = new LinkedList<RenderedFileSystemEntry>();

            double previousAspect = -1;

            // copy entries
            // TODO: try to implement without copying into entries
            var remainingEntries = new LinkedList<IFileSystemEntry>(fileSystemEntries.Where(f => f.TotalSize > 0));
            var result = new List<RenderedFileSystemEntry>();
            while (remainingEntries.Any())
            {
                // remove first entry
                IFileSystemEntry entry = remainingEntries.First.Value;
                remainingEntries.RemoveFirst();

                // add to the current streak
                streakCandidate.AddLast(new RenderedFileSystemEntry(parent)
                {
                    FileSystemEntry = entry
                });

                // real size of the streak
                var streakSizeInBytes = streakCandidate.Sum(f => f.FileSystemEntry.TotalSize);

                // e.g. draw total size = 10 pixels
                var streakBounds = renderer.FillOneDimension(bounds, drawVertically, streakSizeInBytes);

                foreach (var f in streakCandidate)
                {
                    f.Bounds = renderer.FillProportionally(streakBounds, drawVertically, f.FileSystemEntry.TotalSize);
                }

                var aspects = streakCandidate.Select(s => s.Bounds.Size.AspectRatio);
                var worseAspect = aspects.Max();

                // is the new aspect worse?
                if (previousAspect >= 0 && previousAspect < worseAspect)
                {
                    // it got worse
                    // remove the last item
                    streakCandidate.RemoveLast();

                    // put back in entries
                    remainingEntries.AddFirst(entry);

                    // render streak
                    // recalculate streak etc (TODO: this is duplication)
                    streakSizeInBytes = streakCandidate.Sum(f => f.FileSystemEntry.TotalSize);
                    streakBounds = renderer.FillOneDimension(bounds, drawVertically, streakSizeInBytes);
                    foreach (var f in streakCandidate)
                    {
                        f.Bounds = renderer.FillProportionally(streakBounds, drawVertically, f.FileSystemEntry.TotalSize);
                    }

                    result.AddRange(DrawStreak(streakCandidate, streakBounds));

                    // continue in remaining bounds
                    var newList = remainingEntries.ToList();
                    remainingEntries.Clear(); // so that we'll exit the while loop

                    PartialRenderer r = new PartialRenderer(renderer, bounds.Subtract(streakBounds), newList);

                    // add the rest
                    result.AddRange(r.Render(parent));
                }
                else
                {
                    // it got better (or we did not have a previous aspect to compare with)
                    // store it for reference
                    previousAspect = worseAspect;

                    // if it's the last item let's draw
                    if (!remainingEntries.Any())
                    {
                        result.AddRange(DrawStreak(streakCandidate, streakBounds));
                    }
                }
            }

            return result;
        }

        private void Stack(LinkedList<RenderedFileSystemEntry> streak)
        {
            bool isFirst = true;
            OriginD lastOrigin = default(OriginD);
            foreach (var renderedFileSystemEntry in streak)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    renderedFileSystemEntry.Bounds = renderedFileSystemEntry.Bounds.WithOrigin(lastOrigin);
                }

                if (drawVertically)
                {
                    lastOrigin = renderedFileSystemEntry.Bounds.Origin.Move(0, renderedFileSystemEntry.Bounds.Height);
                }
                else
                {
                    lastOrigin = renderedFileSystemEntry.Bounds.Origin.Move(renderedFileSystemEntry.Bounds.Width, 0);
                }
            }
        }

        private IEnumerable<RenderedFileSystemEntry> DrawStreak(LinkedList<RenderedFileSystemEntry> streak, RectangleD streakBounds)
        {
            Stack(streak);

            foreach (var folderWithDrawSize in streak)
            {
                // at least 3 pixels are needed to draw border and content of box
                if (folderWithDrawSize.Bounds.Width < 3 || folderWithDrawSize.Bounds.Height < 3)
                {
                    continue;
                }

                if (folderWithDrawSize == streak.Last.Value)
                {
                    // adjust bounds for last item due to rounding errors etc
                    if (drawVertically)
                    {
                        folderWithDrawSize.Bounds = folderWithDrawSize.Bounds.WithHeight(streakBounds.Bottom - folderWithDrawSize.Bounds.Top);
                    }
                    else
                    {
                        folderWithDrawSize.Bounds = folderWithDrawSize.Bounds.WithWidth(streakBounds.Right - folderWithDrawSize.Bounds.Left);
                    }
                }

                AssertInBounds(streakBounds, folderWithDrawSize.Bounds);

                // subtree
                folderWithDrawSize.Children = renderer.Render(folderWithDrawSize);

                // add in result
                yield return folderWithDrawSize;
            }
        }

        /// <summary>
        /// Asserts that the outer bounds contain the inner bounds.
        /// </summary>
        /// <param name="outerBounds">The outer bounds</param>
        /// <param name="innerBounds">The inner bounds</param>
        private static void AssertInBounds(RectangleD outerBounds, RectangleD innerBounds)
        {
            Debug.Assert(
                innerBounds.Left >= outerBounds.Left && innerBounds.Top >= outerBounds.Top
                && innerBounds.Right <= outerBounds.Right && innerBounds.Bottom <= outerBounds.Bottom,
                $"Rectangle {innerBounds} exceeded {outerBounds}");
        }
    }
}
