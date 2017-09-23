using System;

namespace CRLFLabs.ViewSize
{
    public class FileSystemEventArgs<T> : EventArgs
        where T : class, IFileSystemEntry<T>, new()
    {
        public FileSystemEventArgs(T fileSystemEntry)
        {
            FileSystemEntry = fileSystemEntry;
        }

        public T FileSystemEntry
        {
            get;
        }
    }
}
