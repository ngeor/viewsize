using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using CRLFLabs.ViewSize.Drawing;

namespace CRLFLabs.ViewSize.TreeMap
{
    public class FolderWithDrawSize
    {
        public IFileSystemEntry Folder { get; set; }
        public RectangleD DrawSize { get; set; }
        public IList<FolderWithDrawSize> Children { get; set; }
    }

    /// <summary>
    /// Renders a tree map.
    /// </summary>
    class Renderer
    {
        /// <summary>
        /// Renders a tree map of the given file system entries within the given bounds.
        /// </summary>
        /// <param name="fullBounds">The bounds within to render.</param>
        /// <param name="fileSystemEntries">The file system entries.</param>
        public TreeMapDataSource Render(RectangleD fullBounds, IList<IFileSystemEntry> fileSystemEntries)
        {
            // e.g. real total size = 200 bytes
            var realTotalSize = fileSystemEntries.Sum(f => f.TotalSize);

            // e.g. draw total size = 100 pixels = 20x5
            var drawTotalSize = fullBounds.Width * fullBounds.Height;

            var conversions = new Conversions
            {
                PixelSize = new PixelArea(drawTotalSize),
                ByteSize = realTotalSize
            };

            var result = new TreeMapDataSource
            {
                Bounds = fullBounds,
                FoldersWithDrawSize = new List<FolderWithDrawSize>()
            };

            Render(fullBounds, fileSystemEntries, conversions, result.FoldersWithDrawSize);

            return result;
        }

        private void Render(RectangleD bounds, IList<IFileSystemEntry> fileSystemEntries, Conversions conversions, IList<FolderWithDrawSize> result)
        {
            bool drawVertically = bounds.Width > bounds.Height;

            var streakCandidate = new LinkedList<FolderWithDrawSize>();

            double previousAspect = -1;

            // copy entries
            var entries = new LinkedList<IFileSystemEntry>(fileSystemEntries.Where(f => f.TotalSize > 0));

            while (entries.Any())
            {
                // remove first entry
                IFileSystemEntry entry = entries.First.Value;
                entries.RemoveFirst();

                // add to the current streak
                streakCandidate.AddLast(new FolderWithDrawSize
                {
                    Folder = entry
                });

                // real size of the streak
                var realStreakSize = streakCandidate.Sum(f => f.Folder.TotalSize);

                // e.g. draw total size = 10 pixels
                var drawStreakSize = conversions.FillOneDimension(bounds, drawVertically, realStreakSize);

                foreach (var f in streakCandidate)
                {
                    f.DrawSize = conversions.FillProportionally(drawStreakSize, drawVertically, f.Folder.TotalSize);
                }

                var aspects = streakCandidate.Select(s => s.DrawSize.Size.AspectRatio);
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
                    drawStreakSize = conversions.FillOneDimension(bounds, drawVertically, realStreakSize);
                    foreach (var f in streakCandidate)
                    {
                        f.DrawSize = conversions.FillProportionally(drawStreakSize, drawVertically, f.Folder.TotalSize);
                    }


                    DrawStreak(streakCandidate, drawStreakSize, drawVertically, conversions, result);

                    // continue in remaining bounds
                    var newList = entries.ToList();
                    entries.Clear();
                    Render(bounds.Subtract(drawStreakSize), newList, conversions, result);
                }
                else
                {
                    // it got better (or we did not have a previous aspect to compare with)
                    // store it for reference
                    previousAspect = worseAspect;

                    // if it's the last item let's draw
                    if (!entries.Any())
                    {
                        DrawStreak(streakCandidate, drawStreakSize, drawVertically, conversions, result);
                    }
                }
            }
        }

        private void Stack(LinkedList<FolderWithDrawSize> streakCandidate, bool drawVertically)
        {
            bool isFirst = true;
            OriginD lastOrigin = default(OriginD);
            foreach (var x in streakCandidate)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    x.DrawSize = x.DrawSize.WithOrigin(lastOrigin);
                }

                if (drawVertically)
                {
                    lastOrigin = x.DrawSize.Origin.Move(0, x.DrawSize.Height);
                }
                else
                {
                    lastOrigin = x.DrawSize.Origin.Move(x.DrawSize.Width, 0);
                }
            }
        }

        private void DrawStreak(LinkedList<FolderWithDrawSize> streakCandidate, RectangleD bounds, bool drawVertically, Conversions conversions, IList<FolderWithDrawSize> result)
        {
            Stack(streakCandidate, drawVertically);

            foreach (var s in streakCandidate)
            {
                // at least 3 pixels are needed to draw border and content of box
                if (s.DrawSize.Width < 3 || s.DrawSize.Height < 3)
                {
                    continue;
                }

                if (s == streakCandidate.Last.Value)
                {
                    // adjust bounds for last item due to rounding errors etc
                    if (drawVertically)
                    {
                        s.DrawSize = s.DrawSize.WithHeight(bounds.Bottom - s.DrawSize.Top);
                    }
                    else
                    {
                        s.DrawSize = s.DrawSize.WithWidth(bounds.Right - s.DrawSize.Left);
                    }
                }

                AssertInBounds(bounds, s.DrawSize);

                // add in result
                result.Add(s);

                // subtree
                s.Children = new List<FolderWithDrawSize>();
                Render(s.DrawSize, s.Folder.Children, conversions, s.Children);
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
