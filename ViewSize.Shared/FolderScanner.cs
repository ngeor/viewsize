namespace CRLFLabs.ViewSize
{
    public class FolderScanner
    {
        public FileEntry Root
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether the user has requested to cancel the scan.
        /// </summary>
        /// <value><c>true</c> if cancel requested; otherwise, <c>false</c>.</value>
        public bool CancelRequested
        {
            get;
            private set;
        }

        public void Cancel()
        {
            CancelRequested = true;
        }

        public void Scan(string path)
        {
            Root = new FileEntry(this, path);
            Root.Calculate();
        }

        // TODO scan duration
        // TODO events
    }
}
