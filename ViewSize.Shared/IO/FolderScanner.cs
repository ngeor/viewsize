using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CRLFLabs.ViewSize.IO
{
    /// <summary>
    /// Scans a folder recursively and gathers information.
    /// This is the main API that should be used.
    /// </summary>
    public class FolderScanner : IFolderScanner
    {
        /// <summary>
        /// Holds a value indicating whether scanning is in progress.
        /// </summary>
        private bool scanning;

        public FolderScanner(IFileUtils fileUtils)
        {
            FileUtils = fileUtils;
        }

        private IFileUtils FileUtils { get; }

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
                    var root = new FileSystemEntry(path, null);
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

                // reset cancel flag
                CancelRequested = false;
            }
        }

        private void Calculate(FileSystemEntry entry)
        {
            entry.DisplayText = entry.IsTopLevel ? entry.Path : Path.GetFileName(entry.Path);

            // calculate children recursively
            CalculateChildren(entry);
        }

        private void CalculateChildren(FileSystemEntry entry)
        {
            if (CancelRequested)
            {
                return;
            }

            var paths = FileUtils.EnumerateFileSystemEntries(entry.Path);
            entry.IsDirectory = paths != null;
            if (!entry.IsDirectory)
            {
                // just a regular file, not a directory
                // my file size (should be zero for directories)
                entry.OwnSize = FileUtils.FileLength(entry.Path);
                entry.AdjustTotalSizeAndSortChildren();
                return;
            }

            // only fire for directories because otherwise it's too slow
            FireScanning(entry);

            // a directory after all

            // Important: just loop over paths, don't convert to list
            // you don't know how many they are, and you need to be able to cancel the process!
            foreach (var path in paths)
            {
                if (CancelRequested)
                {
                    break;
                }

                // constructor links to parent's children colletion
                var child = new FileSystemEntry(path, entry);

                // recursion is done here
                Calculate(child);
            }

            // now that children are done, we can calculate total size
            entry.AdjustTotalSizeAndSortChildren();
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
