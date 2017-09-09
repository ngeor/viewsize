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
        private readonly List<IFileSystemEntry> topLevelFolders = new List<IFileSystemEntry>();
        private DateTime startScan;
        private DateTime stopScan;
        private bool scanning;


        public IList<IFileSystemEntry> TopLevelFolders => topLevelFolders;

        public long TotalSize => topLevelFolders.Select(f=>f.TotalSize).Sum();

        /// <summary>
        /// Gets a value indicating whether the user has requested to cancel the scan.
        /// </summary>
        /// <value><c>true</c> if cancel requested; otherwise, <c>false</c>.</value>
        public bool CancelRequested
        {
            get;
            private set;
        }

        public TimeSpan Duration => scanning ? DateTime.UtcNow.Subtract(startScan) : stopScan.Subtract(startScan);

        public void Cancel()
        {
            CancelRequested = true;
        }

        public void Scan(string path)
        {
            if (scanning)
            {
                throw new InvalidOperationException("Already scanning");
            }

            scanning = true;
            startScan = DateTime.UtcNow;
            try
            {
                TopLevelFolders.Clear();
                var root = new FileSystemEntry(this, path);
                TopLevelFolders.Add(root);
                root.Calculate();
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

        public event EventHandler<FileSystemEventArgs> Scanning;

        internal void FireScanning(FileSystemEntry folder)
        {
            Scanning?.Invoke(this, new FileSystemEventArgs(folder));
        }
    }
}
