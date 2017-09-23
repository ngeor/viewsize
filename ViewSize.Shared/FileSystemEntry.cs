using CRLFLabs.ViewSize.Drawing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CRLFLabs.ViewSize
{
    public interface IFileSystemEntry<T> where T : IFileSystemEntry<T>
    {
        string Path { get; set; }
        long OwnSize { get; set; }

        // TODO: can be computed
        long TotalSize { get; set; }

        // TODO: can be computed
        double Percentage { get; set; }

        // relationships
        T Parent { get; set; }
        IList<T> Children { get; set; }

        // UI
        string DisplayText { get; set; }
        string DisplaySize { get; set; }

        // TreeMap
        RectangleD Bounds { get; set; }
    }

    public static class FileSystemEntryExtensions
    {
        public static T Find<T>(this T fileSystemEntry, string path)
            where T : IFileSystemEntry<T>
        {
            // TODO optimize this
            if (fileSystemEntry.Path == path)
            {
                return fileSystemEntry;
            }

            return fileSystemEntry.Children.Find(path);
        }

        public static T Find<T>(this IList<T> fileSystemEntries, string path)
            where T : IFileSystemEntry<T>
        {
            // TODO optimize this
            var q = from entry in fileSystemEntries
                    let match = entry.Find(path)
                    where match != null
                    select match;

            return q.FirstOrDefault();
        }

        public static IEnumerable<T> Ancestors<T>(this T fileSystemEntry)
            where T : IFileSystemEntry<T>
        {
            if (fileSystemEntry.Parent == null)
            {
                return Enumerable.Empty<T>();
            }
            else
            {
                return fileSystemEntry.Parent.Ancestors().Concat(Enumerable.Repeat(fileSystemEntry.Parent, 1));
            }
        }

        /// <summary>
        /// Checks if this object is a descendant of the given object.
        /// </summary>
        public static bool IsDescendantOf<T>(this T folderWithDrawSize, T otherFolder)
            where T : class, IFileSystemEntry<T>, new()
        {
            if (otherFolder == null)
            {
                return false;
            }

            for (var n = folderWithDrawSize; n != null; n = n.Parent)
            {
                if (n == otherFolder)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
