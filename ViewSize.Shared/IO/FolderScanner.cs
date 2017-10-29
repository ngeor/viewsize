// <copyright file="FolderScanner.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

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
            this.FileUtils = fileUtils;
        }

        private IFileUtils FileUtils { get; }

        // TODO: remove TotalSize from here, move it to the Model
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
            this.CancelRequested = true;
        }

        /// <summary>
        /// Scans the given path.
        /// </summary>
        /// <param name="paths">The paths to scan.</param>
        public IReadOnlyList<FileSystemEntry> Scan(params string[] paths)
        {
            if (this.scanning)
            {
                throw new InvalidOperationException("Already scanning");
            }

            this.scanning = true;
            this.TotalSize = 0;

            try
            {
                var result = new List<FileSystemEntry>();
                foreach (var path in paths)
                {
                    var root = new FileSystemEntry(path, null);
                    result.Add(root);
                    this.Calculate(root);
                }

                // calculate the total size
                // necessary for percentages etc
                this.TotalSize = result.Select(f => f.TotalSize).Sum();

                // apply properties that depend on that
                this.SetPropertiesAfterScanOnChildren(result);

                return result;
            }
            finally
            {
                this.scanning = false;

                // reset cancel flag
                this.CancelRequested = false;
            }
        }

        private void Calculate(FileSystemEntry entry)
        {
            entry.DisplayText = entry.IsTopLevel ? entry.Path : Path.GetFileName(entry.Path);

            // calculate children recursively
            this.CalculateChildren(entry);
        }

        private void CalculateChildren(FileSystemEntry entry)
        {
            if (this.CancelRequested)
            {
                return;
            }

            var paths = this.FileUtils.EnumerateFileSystemEntries(entry.Path);
            entry.IsDirectory = paths != null;
            if (!entry.IsDirectory)
            {
                // just a regular file, not a directory
                // my file size (should be zero for directories)
                entry.OwnSize = this.FileUtils.FileLength(entry.Path);
                entry.AdjustTotalSizeAndSortChildren();
                return;
            }

            // only fire for directories because otherwise it's too slow
            this.FireScanning(entry);

            // a directory after all

            // Important: just loop over paths, don't convert to list
            // you don't know how many they are, and you need to be able to cancel the process!
            foreach (var path in paths)
            {
                if (this.CancelRequested)
                {
                    break;
                }

                // constructor links to parent's children colletion
                var child = new FileSystemEntry(path, entry);

                // recursion is done here
                this.Calculate(child);
            }

            // now that children are done, we can calculate total size
            entry.AdjustTotalSizeAndSortChildren();
        }

        #region Setting properties after scan is complete
        private void SetPropertiesAfterScanRecursively(FileSystemEntry entry)
        {
            this.SetPropertiesAfterScan(entry);
            this.SetPropertiesAfterScanOnChildren(entry.Children);
        }

        private void SetPropertiesAfterScanOnChildren(IEnumerable<FileSystemEntry> entries)
        {
            foreach (var child in entries)
            {
                this.SetPropertiesAfterScanRecursively(child);
            }
        }

        private void SetPropertiesAfterScan(FileSystemEntry entry)
        {
            entry.Percentage = (double)entry.TotalSize / this.TotalSize;
            entry.DisplaySize = this.FileUtils.FormatBytes(entry.TotalSize) + $" ({entry.Percentage:P2})";
        }
        #endregion

        #region Events
        public event EventHandler<FileSystemEventArgs> Scanning;

        internal void FireScanning(FileSystemEntry folder)
        {
            this.Scanning?.Invoke(this, new FileSystemEventArgs(folder));
        }
        #endregion
    }
}
