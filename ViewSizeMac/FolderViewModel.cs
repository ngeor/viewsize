using Foundation;
using CRLFLabs.ViewSize;
using System.Collections.Generic;
using System.Linq;

namespace ViewSizeMac
{
    class FolderViewModel : NSObject
    {
        public FolderViewModel(Folder fileEntry)
        {
            FileEntry = fileEntry;
        }

        private Folder FileEntry { get; }

        public List<FolderViewModel> Children => FileEntry.Children.Select(c => new FolderViewModel(c))
                                                          .OrderByDescending(c => c.TotalSize)
                                                          .ToList();

        public string Path => FileEntry.Path;

        public string FileName => System.IO.Path.GetFileName(Path);

        public long TotalSize => FileEntry.TotalSize;

        public bool IsRoot => FileEntry.IsRoot;

        public double Percentage => FileEntry.Percentage;
    }
}
