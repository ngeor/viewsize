using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using CRLFLabs.ViewSize.Drawing;

namespace CRLFLabs.ViewSize.TreeMap
{
    delegate void DoRender(FolderWithDrawSize folderWithDrawSize);

    public class FolderWithDrawSize
    {
        public IFileSystemEntry Folder { get; set; }
        public RectangleD DrawSize { get; set; }
    }

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
        /// <param name="fullBounds">The bounds within to render.</param>
        /// <param name="fileSystemEntries">The file system entries.</param>
        public void Render(RectangleD fullBounds, IList<IFileSystemEntry> fileSystemEntries)
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

            Render(fullBounds, fileSystemEntries, conversions);
        }

        private void Render(RectangleD bounds, IList<IFileSystemEntry> fileSystemEntries, Conversions conversions)
        {
            Debug.WriteLine($"Render bounds={bounds} number of fs entries={fileSystemEntries.Count}");

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

                Debug.WriteLine($"DrawStreakSize = {drawStreakSize}");

                foreach (var f in streakCandidate)
                {
                    f.DrawSize = conversions.FillProportionally(drawStreakSize, drawVertically, f.Folder.TotalSize);
                    Debug.WriteLine($"Calculated rect {f.DrawSize}");
                }

                var aspects = streakCandidate.Select(s => s.DrawSize.Size.AspectRatio);
                var worseAspect = aspects.Max();

                // is the new aspect worse?
                if (previousAspect >= 0 && previousAspect < worseAspect)
                {
                    Debug.WriteLine("backtracking");

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


                    DrawStreak(streakCandidate, drawStreakSize, drawVertically, conversions);

                    // continue in remaining bounds
                    var newList = entries.ToList();
                    entries.Clear();
                    Render(bounds.Subtract(drawStreakSize), newList, conversions);
                }
                else
                {
                    // it got better (or we did not have a previous aspect to compare with)
                    // store it for reference
                    previousAspect = worseAspect;

                    // if it's the last item let's draw
                    if (!entries.Any())
                    {
                        Debug.WriteLine("rendering due to empty list");
                        DrawStreak(streakCandidate, drawStreakSize, drawVertically, conversions);
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

        private void DrawStreak(LinkedList<FolderWithDrawSize> streakCandidate, RectangleD bounds, bool drawVertically, Conversions conversions)
        {
            Debug.WriteLine("Draw streak within {0}", bounds);

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

                DoRender?.Invoke(s);

                // subtree
                Render(s.DrawSize, s.Folder.Children, conversions);
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
