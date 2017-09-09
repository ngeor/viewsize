using System;
using System.Collections.Generic;
using System.Linq;

namespace CRLFLabs.ViewSize
{
    public interface IFileSystemEntry
    {
        string Path { get; }
        long TotalSize { get; }
        IList<IFileSystemEntry> Children { get; }
        double Percentage { get; }
        string DisplayText { get; }
        string DisplaySize { get; }
    }

    /// <summary>
    /// Represents a file or folder.
    /// </summary>
    public class FileSystemEntry : IFileSystemEntry
    {
        public FileSystemEntry(FolderScanner folderScanner, string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Path cannot be empty", nameof(path));
            }

            if (folderScanner == null)
            {
                throw new ArgumentNullException(nameof(folderScanner));
            }

            Path = path;
            FolderScanner = folderScanner;
        }

        public string Path { get; }
        public long OwnSize { get; private set; }
        public long TotalSize { get; private set; }
        public IList<IFileSystemEntry> Children { get; private set; }
        public double Percentage => (double)TotalSize / FolderScanner.TotalSize;
        private bool IsRoot => FolderScanner.IsRoot(this);

        public string DisplayText => IsRoot ? Path : System.IO.Path.GetFileName(Path);
        public string DisplaySize => FileUtils.FormatBytes(TotalSize) + $" ({Percentage:P2})";

        private FolderScanner FolderScanner { get; }

        public void Calculate()
        {
            if (FolderScanner.CancelRequested)
            {
                return;
            }

            FolderScanner.FireScanning(this);

            // my file size (should be zero for directories)
            OwnSize = FileUtils.FileLength(Path);

            // calculate children recursively
            Children = CalculateChildren().Select(c => (IFileSystemEntry)c).ToList();
        }

        private List<FileSystemEntry> CalculateChildren()
        {
            var result = FileUtils.EnumerateFileSystemEntries(Path).Select(p => new FileSystemEntry(FolderScanner, p)).ToList();
            foreach (var child in result)
            {
                // recursion is done here
                child.Calculate();
            }

            // now that children are done, we can calculate total size
            TotalSize = OwnSize + result.Select(c => c.TotalSize).Sum();
            result.Sort((x, y) =>
            {
                if (x.TotalSize > y.TotalSize)
                {
                    return -1;
                }
                else if (x.TotalSize < y.TotalSize)
                {
                    return 1;
                }
                else
                {
                    return x.Path.CompareTo(y.Path);
                }
            });

            return result;
        }
    }
}
