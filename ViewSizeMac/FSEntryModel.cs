using Foundation;
using CRLFLabs.ViewSize;
using System.Collections.Generic;
using System.Linq;

namespace ViewSizeMac
{
    public class FSEntryModel : NSObject
    {
        public FSEntryModel(IFileSystemEntry fileSystemEntry, FSEntryModel parent)
        {
            FileSystemEntry = fileSystemEntry;

            Parent = parent;

            // this one needs to be done like this and not on the fly,
            // otherwise the app suffers a native crash
            Children = FSEntryModel.ToModels(fileSystemEntry.Children, this);
        }

        private IFileSystemEntry FileSystemEntry { get; }

        public IList<FSEntryModel> Children { get; }

        public FSEntryModel Parent { get; }

        public string Path => FileSystemEntry.Path;

        public long TotalSize => FileSystemEntry.TotalSize;

        public double Percentage => FileSystemEntry.Percentage;

        public string DisplayText => FileSystemEntry.DisplayText;

        public string DisplaySize => FileSystemEntry.DisplaySize;

        public static IList<FSEntryModel> ToModels(IEnumerable<IFileSystemEntry> fileSystemEntries, FSEntryModel parent)
        {
            return fileSystemEntries.Select(f => new FSEntryModel(f, parent)).ToList();
        }

        public FSEntryModel Find(string path)
        {
            // TODO optimize this
            if (Path == path)
            {
                return this;
            }

            foreach (var child in Children)
            {
                var result = child.Find(path);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        public IEnumerable<FSEntryModel> Ancestors()
        {
            if (Parent == null)
            {
                return Enumerable.Empty<FSEntryModel>();
            }
            else
            {
                return Parent.Ancestors().Concat(Enumerable.Repeat(Parent, 1));
            }
        }
    }
}
