using System;
using System.Collections.Generic;
using CRLFLabs.ViewSize;
using CRLFLabs.ViewSize.Drawing;

namespace ViewSize.Tests
{
    public class FileSystemEntry : IFileSystemEntry<FileSystemEntry>
    {
        public string Path { get; set; }
        public long TotalSize { get; set; }
        public long OwnSize { get; set; }
        public double Percentage { get; set; }
        public string DisplayText { get; set; }
        public string DisplaySize { get; set; }
        public FileSystemEntry Parent { get; set; }
        public IList<FileSystemEntry> Children { get; set; }
        public RectangleD Bounds { get; set; }
    }
}
