using System;

namespace CRLFLabs.ViewSize
{
    /// <summary>
    /// Scans a folder recursively and gathers information.
    /// This is the main API that should be used.
    /// </summary>
    public class FolderScanner
    {
        private DateTime startScan;
        private DateTime stopScan;
        private bool scanning;

        public Folder Root
        {
            get;
            private set;
        }

        public long TotalSize => Root?.TotalSize ?? 0;

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

                Root = new Folder(this, path);
                Root.Calculate();
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
        internal bool IsRoot(Folder folder)
        {
            return folder != null && folder == Root;
        }

        public event EventHandler<FolderEventArgs> Scanning;

        internal void FireScanning(Folder folder)
        {
            Scanning?.Invoke(this, new FolderEventArgs(folder));
        }
    }
}
