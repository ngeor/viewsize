using System;
using System.ComponentModel;
using CRLFLabs.ViewSize.IO;
using System.Collections.Generic;

namespace CRLFLabs.ViewSize.Mvp
{
    public class MainModel : IMainModel
    {
        public const string IsScanningPropertyName = "IsScanning";
        public const string SortKeyPropertyName = "SortKey";
        public const string ChildrenPropertyName = "Children";
        public const string SelectedPropertyName = "Selected";
        public const string FolderPropertyName = "Folder";
        public const string TopLevelFoldersPropertyName = "TopLevelFolders";

        private bool _isScanning;
        private string _folder;
        private SortKey _sortKey;
        private IReadOnlyList<FileSystemEntry> _children;
        private FileSystemEntry _selected;
        private IReadOnlyList<FileSystemEntry> _topLevelFolders;

        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsScanning
        {
            get => _isScanning;
            set
            {
                if (_isScanning != value)
                {
                    PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(IsScanningPropertyName));
                    _isScanning = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(IsScanningPropertyName));
                }
            }
        }

        public string Folder
        {
            get => _folder;
            set
            {
                if (_folder != value)
                {
                    PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(FolderPropertyName));
                    _folder = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(FolderPropertyName));
                }
            }
        }

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

        public IReadOnlyList<FileSystemEntry> TopLevelFolders
        {
            get => _topLevelFolders;
            set
            {
                if (_topLevelFolders != value)
                {
                    PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(TopLevelFoldersPropertyName));
                    _topLevelFolders = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(TopLevelFoldersPropertyName));
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
