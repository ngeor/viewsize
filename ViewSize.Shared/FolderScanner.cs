using System;
using System.Collections.Generic;
using System.Linq;

namespace CRLFLabs.ViewSize
{
    /// <summary>
    /// Scans a folder recursively and gathers information.
    /// This is the main API that should be used.
    /// </summary>
    public class FolderScanner
    {
        /// <summary>
        /// Holds the list of top level folders that will be scanned.
        /// </summary>
        private readonly List<IFileSystemEntry> topLevelFolders = new List<IFileSystemEntry>();

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
        /// Gets the list of top level folders that will be scanned.
        /// </summary>
        public IList<IFileSystemEntry> TopLevelFolders => topLevelFolders;

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
        public void Scan(string path)
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
                TopLevelFolders.Clear();
                var root = new FileSystemEntry(this, path);
                TopLevelFolders.Add(root);
                root.Calculate();
                TotalSize = topLevelFolders.Select(f => f.TotalSize).Sum();
            }
            finally
            {
                stopScan = DateTime.UtcNow;
                scanning = false;
            }
        }

        /// <summary>
        /// Checks if the given folder is top level.
        /// </summary>
        internal bool IsRoot(FileSystemEntry folder)
        {
            return folder != null && TopLevelFolders.Contains(folder);
        }

        #region Events
        public event EventHandler<FileSystemEventArgs> Scanning;

        internal void FireScanning(FileSystemEntry folder)
        {
            Scanning?.Invoke(this, new FileSystemEventArgs(folder));
        }
        #endregion
    }
}
