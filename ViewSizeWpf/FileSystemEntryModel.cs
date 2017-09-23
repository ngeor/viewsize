using CRLFLabs.ViewSize;
using CRLFLabs.ViewSize.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ViewSizeWpf
{
    public class FileSystemEntryModel : INotifyPropertyChanged, IFileSystemEntry<FileSystemEntryModel>
    {
        private bool isExpanded;
        private bool isSelected;

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

        public string Path { get; set; }
        public long TotalSize { get; set; }
        public long OwnSize { get; set; }
        public double Percentage { get; set; }
        public string DisplayText { get; set; }
        public string DisplaySize { get; set; }
        public FileSystemEntryModel Parent { get; set; }
        public IList<FileSystemEntryModel> Children { get; set; }
        public RectangleD Bounds { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
