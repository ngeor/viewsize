using System;
using System.ComponentModel;
using CRLFLabs.ViewSize.IO;
using System.Collections.Generic;

namespace CRLFLabs.ViewSize.Mvp
{
    public class MainModel : IMainModel
    {
        public const string SortKeyPropertyName = "SortKey";
        public const string ChildrenPropertyName = "Children";
        public const string SelectedPropertyName = "Selected";

        private SortKey _sortKey;
        private IReadOnlyList<FileSystemEntry> _children;
        private FileSystemEntry _selected;

        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;

        public SortKey SortKey
        {
            get
            {
                return _sortKey;    
            }
            set
            {
                if (_sortKey != value)
                {
                    PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(SortKeyPropertyName));
                    _sortKey = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(SortKeyPropertyName));
                }
            }
        }

        public IReadOnlyList<FileSystemEntry> Children
        {
            get
            {
                return _children;
            }
            set
            {
                if (_children != value)
                {
                    PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(ChildrenPropertyName));
                    _children = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(ChildrenPropertyName));
                }
            }
        }

        public FileSystemEntry Selected
        {
            get
            {
                return _selected;
            }
            set
            {
                if (_selected != value)
                {
                    PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(SelectedPropertyName));
                    _selected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(SelectedPropertyName));
                }
            }
        }
    }
}
