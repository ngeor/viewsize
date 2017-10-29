// <copyright file="PartialRenderer.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CRLFLabs.ViewSize.Drawing;
using CRLFLabs.ViewSize.IO;

namespace CRLFLabs.ViewSize.TreeMap
{
    internal class PartialRenderer
    {
        private readonly Renderer renderer;
        private readonly IReadOnlyList<FileSystemEntry> fileSystemEntries;
        private readonly RectangleD initialBounds;
        private RectangleD bounds;
        private bool drawVertically;

        /// <summary>
        /// Creates an instance of this class.
        /// </summary>
        /// <param name="renderer">The parent renderer.</param>
        /// <param name="bounds">The bounds within rendering is confined.</param>
        /// <param name="fileSystemEntries">The file system entries to render.</param>
        public PartialRenderer(Renderer renderer, RectangleD bounds, IReadOnlyList<FileSystemEntry> fileSystemEntries)
        {
            this.renderer = renderer;
            this.fileSystemEntries = fileSystemEntries;
            this.initialBounds = bounds;
        }

        public void Render()
        {
            int i = 0;

            this.SwitchBounds(this.initialBounds);
            while (i < this.fileSystemEntries.Count)
            {
                // start new streak
                var streakCandidate = new LinkedList<FileSystemEntry>();

                double previousAspect = -1;

                bool gotWorse = false;
                while (i < this.fileSystemEntries.Count && !gotWorse)
                {
                    // go on with current streak as long as it doesn't get worse aspect ratio
                    // take next entry
                    var entry = this.fileSystemEntries[i];
                    i++;

                    if (this.renderer.Measurer(entry) <= 0)
                    {
                        entry.Bounds = default(RectangleD);
                        continue;
                    }

                    // add to the current streak
                    streakCandidate.AddLast(entry);

                    // e.g. draw total size = 10 pixels
                    var streakBounds = this.CalculateStreakBounds(streakCandidate);

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
                        streakBounds = this.CalculateStreakBounds(streakCandidate);
                        this.RenderChildrenOfStreak(streakCandidate);

                        // continue in remaining bounds
                        this.SwitchBounds(this.bounds.Subtract(streakBounds));
                    }
                    else
                    {
                        // it got better (or we did not have a previous aspect to compare with)
                        // store it for reference
                        previousAspect = worseAspect;
                    }
                }

                // if it's the last item let's draw what's left
                if (i >= this.fileSystemEntries.Count)
                {
                    this.RenderChildrenOfStreak(streakCandidate);
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
            var streakSizeInBytes = streakCandidate.Sum(this.renderer.Measurer);

            // e.g. draw total size = 10 pixels
            var streakBounds = this.renderer.FillOneDimension(this.bounds, this.drawVertically, streakSizeInBytes);

            // distribute the streak proportionally within the given bounds
            this.renderer.FillProportionally(streakBounds, this.drawVertically, streakCandidate);

            this.AdjustLastEntrySize(streakBounds, streakCandidate);

            return streakBounds;
        }

        private void SwitchBounds(RectangleD bounds)
        {
            this.bounds = bounds;
            this.drawVertically = bounds.Width > bounds.Height;
        }

        private void AdjustLastEntrySize(RectangleD streakBounds, LinkedList<FileSystemEntry> streakCandidate)
        {
            // adjust bounds for last item due to rounding errors etc
            var folderWithDrawSize = streakCandidate.Last.Value;
            if (this.drawVertically)
            {
                folderWithDrawSize.Bounds = folderWithDrawSize.Bounds.WithHeight(streakBounds.Bottom - folderWithDrawSize.Bounds.Top);
            }
            else
            {
                folderWithDrawSize.Bounds = folderWithDrawSize.Bounds.WithWidth(streakBounds.Right - folderWithDrawSize.Bounds.Left);
            }

            AssertInBounds(streakBounds, folderWithDrawSize.Bounds);
        }

        /// <summary>
        /// Recursively renders the children of this streak.
        /// </summary>
        /// <param name="streak">The current streak.</param>
        private void RenderChildrenOfStreak(LinkedList<FileSystemEntry> streak)
        {
            foreach (var entry in streak)
            {
                // subtree
                if (entry.Bounds.IsEmpty)
                {
                    // it is empty, just nuke the children as well so we don't have previous rectangles appearing
                    this.CopyBoundsToChildren(entry);
                }
                else
                {
                    this.renderer.Render(entry);
                }
            }
        }

        private void CopyBoundsToChildren(FileSystemEntry entry)
        {
            foreach (var child in entry.Children)
            {
                child.Bounds = entry.Bounds;
                this.CopyBoundsToChildren(child);
            }
        }
    }
}
