using Foundation;
using CRLFLabs.ViewSize;
using System.Collections.Generic;
using System.Linq;

namespace ViewSizeMac
{
    public class FolderViewModel : NSObject
    {
        public FolderViewModel(Folder fileEntry)
        {
            FileEntry = fileEntry;
        }

        private Folder FileEntry { get; }

        public IList<FolderViewModel> Children => FolderViewModel.ToModels(FileEntry.Children);

        public string Path => FileEntry.Path;

        public string FileName => System.IO.Path.GetFileName(Path);

        public long TotalSize => FileEntry.TotalSize;

        public bool IsRoot => FileEntry.IsRoot;

        public double Percentage => FileEntry.Percentage;

        public static IList<FolderViewModel> ToModels(IEnumerable<Folder> folders)
        {
            return folders.Select(f => new FolderViewModel(f)).ToList();
        }
    }
}
