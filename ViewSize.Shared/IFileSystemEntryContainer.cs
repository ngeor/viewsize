using System.Collections.Generic;

namespace CRLFLabs.ViewSize
{
    public interface IFileSystemEntryContainer
    {
        IReadOnlyList<FileSystemEntry> Children { get; }
        IFileSystemEntryContainer Parent { get; }
    }
}
