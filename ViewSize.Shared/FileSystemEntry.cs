using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CRLFLabs.ViewSize
{
    public interface IFileSystemEntry
    {
        string Path { get; set; }
        long TotalSize { get; set; }
        long OwnSize { get; set; }
        double Percentage { get; set; }
        string DisplayText { get; set; }
        string DisplaySize { get; set; }
        IFileSystemEntry Parent { get; set; }
        IList<IFileSystemEntry> Children { get; set; }
    }

    public static class FileSystemEntryExtensions
    {
        public static IFileSystemEntry Find(this IFileSystemEntry fileSystemEntry, string path)
        {
            // TODO optimize this
            if (fileSystemEntry.Path == path)
            {
                return fileSystemEntry;
            }

            return fileSystemEntry.Children.Find(path);
        }

        public static IFileSystemEntry Find(this IEnumerable<IFileSystemEntry> fileSystemEntries, string path)
        {
            // TODO optimize this
            var q = from entry in fileSystemEntries
                    let match = entry.Find(path)
                    where match != null
                    select match;

            return q.FirstOrDefault();
        }

        public static IEnumerable<IFileSystemEntry> Ancestors(this IFileSystemEntry fileSystemEntry)
        {
            if (fileSystemEntry.Parent == null)
            {
                return Enumerable.Empty<IFileSystemEntry>();
            }
            else
            {
                return fileSystemEntry.Parent.Ancestors().Concat(Enumerable.Repeat(fileSystemEntry.Parent, 1));
            }
        }

    }
}
