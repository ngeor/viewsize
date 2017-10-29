// <copyright file="IFileUtils.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

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
