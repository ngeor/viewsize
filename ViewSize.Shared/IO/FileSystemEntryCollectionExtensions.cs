using CRLFLabs.ViewSize.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CRLFLabs.ViewSize.IO
{
    public static class FileSystemEntryCollectionExtensions
    {
        /// <summary>
        /// Finds the file system entry that exists within these coordinates.
        /// </summary>
        /// <returns>The matching file system entry.</returns>
        /// <param name="pt">Point.</param>
        public static FileSystemEntry Find(this IEnumerable<FileSystemEntry> fileSystemEntries, PointD pt)
        {
            // TODO optimize this function
            return Find(pt, fileSystemEntries);
        }

        private static FileSystemEntry Find(PointD pt, IEnumerable<FileSystemEntry> fileSystemEntries)
        {
            // find the first folder that contains these coordinates
            var match = fileSystemEntries.FirstOrDefault(f => f.Bounds.Contains(pt));

            // try to find a more specific match in its children, otherwise return the match
            return match == null ? null : (Find(pt, match.Children) ?? match);
        }
    }
}
