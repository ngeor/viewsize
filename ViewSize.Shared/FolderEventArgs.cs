using System;

namespace CRLFLabs.ViewSize
{
    public class FolderEventArgs : EventArgs
    {
        public FolderEventArgs(Folder folder)
        {
            Folder = folder;
        }

        public Folder Folder
        {
            get;
            private set;
        }
    }
}
