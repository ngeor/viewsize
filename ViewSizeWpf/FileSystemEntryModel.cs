using CRLFLabs.ViewSize;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ViewSizeWpf
{
    public class FileSystemEntryModel : INotifyPropertyChanged
    {
        private bool isExpanded;
        private bool isSelected;

        public FileSystemEntryModel(IFileSystemEntry entry, FileSystemEntryModel parent = null)
        {
            Path = entry.Path;
            TotalSize = entry.TotalSize;
            OwnSize = entry.OwnSize;
            Percentage = entry.Percentage;
            DisplayText = entry.DisplayText;
            DisplaySize = entry.DisplaySize;
            Children = Convert(entry.Children, this);
            Parent = parent;
        }

        public bool IsExpanded
        {
            get
            {
                return isExpanded;
            }
            set
            {
                if (isExpanded != value)
                {
                    isExpanded = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsExpanded"));
                }
            }
        }

        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsSelected"));
                }
            }
        }

        public FileSystemEntryModel Parent { get; }

        public string Path { get; }

        public long TotalSize { get; }

        public long OwnSize { get; }

        public IList<FileSystemEntryModel> Children { get; }

        public double Percentage { get; }

        public string DisplayText { get; }

        public string DisplaySize { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public static IList<FileSystemEntryModel> Convert(IEnumerable<IFileSystemEntry> fileSystemEntries, FileSystemEntryModel parent = null)
        {
            return new List<FileSystemEntryModel>(fileSystemEntries.Select(e => new FileSystemEntryModel(e, parent)));
        }

        // TODO optimize
        public static FileSystemEntryModel Find(IList<FileSystemEntryModel> folders, string path)
        {
            foreach (var f in folders)
            {
                if (f.Path == path)
                {
                    return f;
                }

                var r = Find(f.Children, path);
                if (r != null)
                {
                    return r;
                }
            }

            return null;
        }
    }
}
