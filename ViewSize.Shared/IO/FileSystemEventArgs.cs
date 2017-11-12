// <copyright file="FileSystemEventArgs.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

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
