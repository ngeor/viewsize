using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CRLFLabs.ViewSize
{
    /// <summary>
    /// Scans a folder recursively and gathers information.
    /// This is the main API that should be used.
    /// </summary>
    public class FolderScanner<T> where T : class, IFileSystemEntry<T>, new()
    {
        /// <summary>
        /// Holds the time when scanning started.
        /// </summary>
        private DateTime startScan;

        /// <summary>
        /// Holds the time when scanning finished.
        /// </summary>
        private DateTime stopScan;

        /// <summary>
        /// Holds a value indicating whether scanning is in progress.
        /// </summary>
        private bool scanning;

        /// <summary>
        /// Gets the total size, in bytes, of the scanned items.
        /// </summary>
        public long TotalSize { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the user has requested to cancel the scan.
        /// </summary>
        /// <value><c>true</c> if cancel has been requested; otherwise, <c>false</c>.</value>
        public bool CancelRequested
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the duration of the scan.
        /// If the scan is in progress, this is the duration so far.
        /// </summary>
        public TimeSpan Duration => scanning ? DateTime.UtcNow.Subtract(startScan) : stopScan.Subtract(startScan);

        /// <summary>
        /// Requests to terminate scanning.
        /// </summary>
        public void Cancel()
        {
            CancelRequested = true;
        }

        /// <summary>
        /// Scans the given path.
        /// </summary>
        /// <param name="path">The path to scan.</param>
        public IList<T> Scan(string path)
        {
            if (scanning)
            {
                throw new InvalidOperationException("Already scanning");
            }

            scanning = true;
            startScan = DateTime.UtcNow;
            TotalSize = 0;

            try
            {
                var result = new List<T>();
                T root = Create(path, null);
                result.Add(root);
                Calculate(root);

                // calculate the total size
                TotalSize = result.Select(f => f.TotalSize).Sum();

                // apply properties that depend on that
                foreach (var child in result)
                {
                    ApplyProperties(child);
                }

                return result;
            }
            finally
            {
                stopScan = DateTime.UtcNow;
                scanning = false;
            }
        }

        private T Create(string path, T parent)
        {
            return new T
            {
                Path = path,
                Parent = parent
            };
        }

        private void Calculate(T fileSystemEntry)
        {
            if (CancelRequested)
            {
                return;
            }

            FireScanning(fileSystemEntry);

            // my file size (should be zero for directories)
            fileSystemEntry.OwnSize = FileUtils.FileLength(fileSystemEntry.Path);

            // calculate children recursively
            fileSystemEntry.Children = CalculateChildren(fileSystemEntry);
        }

        private List<T> CalculateChildren(T fileSystemEntry)
        {
            var result = FileUtils.EnumerateFileSystemEntries(fileSystemEntry.Path)
                                  .Select(p => Create(p, fileSystemEntry))
                                  .ToList();
            foreach (var child in result)
            {
                // recursion is done here
                Calculate(child);
            }

            // now that children are done, we can calculate total size
            fileSystemEntry.TotalSize = fileSystemEntry.OwnSize + result.Select(c => c.TotalSize).Sum();
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

        private void ApplyProperties(T fileSystemEntry)
        {
            fileSystemEntry.Percentage = (double)fileSystemEntry.TotalSize / TotalSize;
            fileSystemEntry.DisplayText = fileSystemEntry.Parent == null ?
                                      fileSystemEntry.Path : Path.GetFileName(fileSystemEntry.Path);
            fileSystemEntry.DisplaySize = FileUtils.FormatBytes(fileSystemEntry.TotalSize) + $" ({fileSystemEntry.Percentage:P2})";

            foreach (var child in fileSystemEntry.Children)
            {
                ApplyProperties(child);
            }
        }

        #region Events
        public event EventHandler<FileSystemEventArgs<T>> Scanning;

        internal void FireScanning(T folder)
        {
            Scanning?.Invoke(this, new FileSystemEventArgs<T>(folder));
        }
        #endregion
    }
}
