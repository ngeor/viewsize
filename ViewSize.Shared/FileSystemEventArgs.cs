using System;

namespace CRLFLabs.ViewSize
{
    public class FileSystemEventArgs : EventArgs
    {
        public FileSystemEventArgs(IFileSystemEntry fileSystemEntry)
        {
            FileSystemEntry = fileSystemEntry;
        }

        public IFileSystemEntry FileSystemEntry
        {
            get;
            private set;
        }
    }
}
