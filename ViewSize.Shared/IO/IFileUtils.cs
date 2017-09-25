using System;
using System.Collections.Generic;

namespace CRLFLabs.ViewSize.IO
{
    public interface IFileUtils
    {
        bool IsDirectory(string path);
        IEnumerable<string> EnumerateFileSystemEntries(string path);
        long FileLength(string path);
        string FormatBytes(double size);
    }
}
