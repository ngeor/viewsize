using Foundation;
using CRLFLabs.ViewSize;
using System.Collections.Generic;
using System.Linq;
using System;

namespace ViewSizeMac
{
    public class FSEntryModel : NSObject, IFileSystemEntry
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
