﻿using System.Collections.Generic;

namespace CRLFLabs.ViewSize.IO
{
    public interface IFileSystemEntryContainer
    {
        IReadOnlyList<FileSystemEntry> Children { get; }
        IFileSystemEntryContainer Parent { get; }
    }
}