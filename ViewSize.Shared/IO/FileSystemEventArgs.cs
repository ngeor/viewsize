using System;

namespace CRLFLabs.ViewSize.IO
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
        }
    }
}
