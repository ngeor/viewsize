// <copyright file="MainModel.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.ComponentModel;
using CRLFLabs.ViewSize.IO;

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

        private bool isScanning;
        private string folder;
        private SortKey sortKey;
        private IReadOnlyList<FileSystemEntry> children;
        private FileSystemEntry selected;
        private IReadOnlyList<FileSystemEntry> topLevelFolders;

        public event PropertyChangingEventHandler PropertyChanging;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsScanning
        {
            get => isScanning;
            set
            {
                if (isScanning != value)
                {
                    PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(IsScanningPropertyName));
                    isScanning = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(IsScanningPropertyName));
                }
            }
        }

        public string Folder
        {
            get => folder;
            set
            {
                if (folder != value)
                {
                    PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(FolderPropertyName));
                    folder = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(FolderPropertyName));
                }
            }
        }

        public SortKey SortKey
        {
            get
            {
                return sortKey;
            }

            set
            {
                if (sortKey != value)
                {
                    PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(SortKeyPropertyName));
                    sortKey = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(SortKeyPropertyName));
                }
            }
        }

        public IReadOnlyList<FileSystemEntry> TopLevelFolders
        {
            get => topLevelFolders;
            set
            {
                if (topLevelFolders != value)
                {
                    PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(TopLevelFoldersPropertyName));
                    topLevelFolders = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(TopLevelFoldersPropertyName));
                }
            }
        }

        public IReadOnlyList<FileSystemEntry> Children
        {
            get
            {
                return children;
            }

            set
            {
                if (children != value)
                {
                    PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(ChildrenPropertyName));
                    children = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(ChildrenPropertyName));
                }
            }
        }

        public FileSystemEntry Selected
        {
            get
            {
                return selected;
            }

            set
            {
                if (selected != value)
                {
                    PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(SelectedPropertyName));
                    selected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(SelectedPropertyName));
                }
            }
        }
    }
}
