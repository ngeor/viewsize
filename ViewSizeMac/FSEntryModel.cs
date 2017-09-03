using Foundation;
using CRLFLabs.ViewSize;
using System.Collections.Generic;
using System.Linq;

namespace ViewSizeMac
{
    public class FSEntryModel : NSObject
    {
        public FSEntryModel(Folder fileSystemEntry)
        {
            FileSystemEntry = fileSystemEntry;

            // this one needs to be done like this and not on the fly,
            // otherwise the app suffers a native crash
            Children = FSEntryModel.ToModels(fileSystemEntry.Children);
        }

        private Folder FileSystemEntry { get; }

        public IList<FSEntryModel> Children { get; } 

        public string Path => FileSystemEntry.Path;

        public long TotalSize => FileSystemEntry.TotalSize;

        public bool IsRoot => FileSystemEntry.IsRoot;

        public double Percentage => FileSystemEntry.Percentage;

        public string DisplayText => FileSystemEntry.DisplayText;

        public string DisplaySize => FileSystemEntry.DisplaySize;

        public static IList<FSEntryModel> ToModels(IEnumerable<Folder> fileSystemEntries)
        {
            return fileSystemEntries.Select(f => new FSEntryModel(f)).ToList();
        }
    }
}
