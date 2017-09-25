using System;
using System.Collections.Generic;

namespace CRLFLabs.ViewSize.IO
{
    public interface IFolderScanner
    {
        event EventHandler<FileSystemEventArgs> Scanning;

        IReadOnlyList<FileSystemEntry> Scan(params string[] paths);
        void Cancel();

    }
}
