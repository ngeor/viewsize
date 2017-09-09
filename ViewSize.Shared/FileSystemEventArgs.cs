using System;

namespace CRLFLabs.ViewSize
{
    public class FileSystemEventArgs : EventArgs
    {
        public FileSystemEventArgs(FileSystemEntry fileSystemEntry)
        {
            FileSystemEntry = fileSystemEntry;
        }

        public FileSystemEntry FileSystemEntry
        {
            get;
            private set;
        }
    }
}
