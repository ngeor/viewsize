using System;
using System.Collections.Generic;
using CRLFLabs.ViewSize;

namespace ViewSize.Tests
{
    public class FileSystemEntry : IFileSystemEntry
    {
        public string Path { get; set; }
        public long TotalSize { get; set; }
        public long OwnSize { get; set; }
        public double Percentage { get; set; }
        public string DisplayText { get; set; }
        public string DisplaySize { get; set; }
        public IFileSystemEntry Parent { get; set; }
        public IList<IFileSystemEntry> Children { get; set; }
    }
}
