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

            foreach (var child in fileSystemEntry.Children)
            {
                var result = child.Find(path);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
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
