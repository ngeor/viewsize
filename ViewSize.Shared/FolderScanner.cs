using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CRLFLabs.ViewSize.IO;

namespace CRLFLabs.ViewSize
{
    /// <summary>
    /// Scans a folder recursively and gathers information.
    /// This is the main API that should be used.
    /// </summary>
    public class FolderScanner : IFileSystemEntryContainer
    {
        /// <summary>
        /// Holds a value indicating whether scanning is in progress.
        /// </summary>
        private bool scanning;

        /// <summary>
        /// Gets the total size, in bytes, of the scanned items.
        /// </summary>
        private long TotalSize { get; set; }

        /// <summary>
        /// Gets a value indicating whether the user has requested to cancel the scan.
        /// </summary>
        /// <value><c>true</c> if cancel has been requested; otherwise, <c>false</c>.</value>
        public bool CancelRequested
        {
            get;
            private set;
        }

        public IReadOnlyList<FileSystemEntry> Children => throw new NotImplementedException();

        public IFileSystemEntryContainer Parent => null;

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
        /// <param name="paths">The paths to scan.</param>
        public IReadOnlyList<FileSystemEntry> Scan(params string[] paths)
        {
            if (scanning)
            {
                throw new InvalidOperationException("Already scanning");
            }

            scanning = true;
            TotalSize = 0;

            try
            {
                var result = new List<FileSystemEntry>();
                foreach (var path in paths)
                {
                    var root = new FileSystemEntry(path, this);
                    result.Add(root);
                    Calculate(root);
                }

                // calculate the total size
                // necessary for percentages etc
                TotalSize = result.Select(f => f.TotalSize).Sum();

                // apply properties that depend on that
                SetPropertiesAfterScanOnChildren(result);

                return result;
            }
            finally
            {
                scanning = false;
            }
        }

        private void Calculate(FileSystemEntry entry)
        {
            if (CancelRequested)
            {
                return;
            }

            entry.DisplayText = entry.IsTopLevel ? entry.Path : Path.GetFileName(entry.Path);

            // calculate children recursively
            entry.Children = CalculateChildren(entry);
        }

        private List<FileSystemEntry> CalculateChildren(FileSystemEntry entry)
        {
            var paths = FileUtils.EnumerateFileSystemEntries(entry.Path);
            entry.IsDirectory = paths != null;
            if (!entry.IsDirectory)
            {
                // just a regular file, not a directory
                // my file size (should be zero for directories)
                entry.OwnSize = FileUtils.FileLength(entry.Path);
                entry.TotalSize = entry.OwnSize;
                return new List<FileSystemEntry>();
            }

            // only fire for directories because otherwise it's too slow
            FireScanning(entry);

            // a directory after all
            var result = (
                from path in paths
                select new FileSystemEntry(path, entry)
            ).ToList();

            foreach (var child in result)
            {
                // recursion is done here
                Calculate(child);
            }

            // now that children are done, we can calculate total size
            entry.TotalSize = result.Select(c => c.TotalSize).Sum();
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

        #region Setting properties after scan is complete
        private void SetPropertiesAfterScanRecursively(FileSystemEntry entry)
        {
            SetPropertiesAfterScan(entry);
            SetPropertiesAfterScanOnChildren(entry.Children);
        }

        private void SetPropertiesAfterScanOnChildren(IEnumerable<FileSystemEntry> entries)
        {
            foreach (var child in entries)
            {
                SetPropertiesAfterScanRecursively(child);
            }
        }

        private void SetPropertiesAfterScan(FileSystemEntry entry)
        {
            entry.Percentage = (double)entry.TotalSize / TotalSize;
            entry.DisplaySize = FileUtils.FormatBytes(entry.TotalSize) + $" ({entry.Percentage:P2})";
        }
        #endregion

        #region Events
        public event EventHandler<FileSystemEventArgs> Scanning;

        internal void FireScanning(FileSystemEntry folder)
        {
            Scanning?.Invoke(this, new FileSystemEventArgs(folder));
        }
        #endregion
    }
}
