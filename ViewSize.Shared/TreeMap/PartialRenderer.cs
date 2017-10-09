using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CRLFLabs.ViewSize.Drawing;
using CRLFLabs.ViewSize.IO;

namespace CRLFLabs.ViewSize.TreeMap
{
    class PartialRenderer
    {
        private readonly Renderer _renderer;
        private readonly IReadOnlyList<FileSystemEntry> _fileSystemEntries;
        private readonly RectangleD _initialBounds;
        private RectangleD _bounds;
        private bool _drawVertically;

        /// <summary>
        /// Creates an instance of this class.
        /// </summary>
        /// <param name="renderer">The parent renderer.</param>
        /// <param name="bounds">The bounds within rendering is confined.</param>
        /// <param name="fileSystemEntries">The file system entries to render.</param>
        public PartialRenderer(Renderer renderer, RectangleD bounds, IReadOnlyList<FileSystemEntry> fileSystemEntries)
        {
            _renderer = renderer;
            _fileSystemEntries = fileSystemEntries;
            _initialBounds = bounds;
        }

        public void Render()
        {
            int i = 0;

            SwitchBounds(_initialBounds);
            while (i < _fileSystemEntries.Count)
            {
                // start new streak
                var streakCandidate = new LinkedList<FileSystemEntry>();

                double previousAspect = -1;

                bool gotWorse = false;
                while (i < _fileSystemEntries.Count && !gotWorse)
                {
                    // go on with current streak as long as it doesn't get worse aspect ratio
                    // take next entry
                    var entry = _fileSystemEntries[i];
                    i++;

                    if (entry.TotalSize <= 0)
                    {
                        continue;
                    }

                    // add to the current streak
                    streakCandidate.AddLast(entry);

                    // e.g. draw total size = 10 pixels
                    var streakBounds = CalculateStreakBounds(streakCandidate);

                    var aspects = streakCandidate.Select(s => s.Bounds.Size.AspectRatio);
                    var worseAspect = aspects.Max();

                    // is the new aspect worse?
                    gotWorse = previousAspect >= 0 && previousAspect < worseAspect;
                    if (gotWorse)
                    {
                        // it got worse
                        // remove the last item
                        streakCandidate.RemoveLast();
                        i--;

                        // render streak
                        // recalculate streak bounds
                        streakBounds = CalculateStreakBounds(streakCandidate);
                        RenderChildrenOfStreak(streakCandidate);

                        // continue in remaining bounds
                        SwitchBounds(_bounds.Subtract(streakBounds));
                    }
                    else
                    {
                        // it got better (or we did not have a previous aspect to compare with)
                        // store it for reference
                        previousAspect = worseAspect;
                    }
                }
                
                // if it's the last item let's draw what's left
                if (i >= _fileSystemEntries.Count)
                {
                    RenderChildrenOfStreak(streakCandidate);
                }
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

        private RectangleD CalculateStreakBounds(LinkedList<FileSystemEntry> streakCandidate)
        {
            // real size of the streak
            var streakSizeInBytes = streakCandidate.Sum(f => f.TotalSize);

            // e.g. draw total size = 10 pixels
            var streakBounds = _renderer.FillOneDimension(_bounds, _drawVertically, streakSizeInBytes);

            // distribute the streak proportionally within the given bounds
            _renderer.FillProportionally(streakBounds, _drawVertically, streakCandidate);

            AdjustLastEntrySize(streakBounds, streakCandidate);

            return streakBounds;
        }

        private void SwitchBounds(RectangleD bounds)
        {
            _bounds = bounds;
            _drawVertically = bounds.Width > bounds.Height;
        }

        private void AdjustLastEntrySize(RectangleD streakBounds, LinkedList<FileSystemEntry> streakCandidate)
        {
            // adjust bounds for last item due to rounding errors etc
            var folderWithDrawSize = streakCandidate.Last.Value;
            if (_drawVertically)
            {
                folderWithDrawSize.Bounds = folderWithDrawSize.Bounds.WithHeight(streakBounds.Bottom - folderWithDrawSize.Bounds.Top);
            }
            else
            {
                folderWithDrawSize.Bounds = folderWithDrawSize.Bounds.WithWidth(streakBounds.Right - folderWithDrawSize.Bounds.Left);
            }

            AssertInBounds(streakBounds, folderWithDrawSize.Bounds);
        }

        private void RenderChildrenOfStreak(LinkedList<FileSystemEntry> streak)
        {
            foreach (var entry in streak)
            {
                // subtree
                _renderer.Render(entry);
            }
        }
    }
}
