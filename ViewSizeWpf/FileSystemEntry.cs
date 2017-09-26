using CRLFLabs.ViewSize;
using CRLFLabs.ViewSize.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace CRLFLabs.ViewSize.IO
{
    partial class FileSystemEntry : INotifyPropertyChanged
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

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
