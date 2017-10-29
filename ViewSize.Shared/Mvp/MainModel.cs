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
            get => this.isScanning;
            set
            {
                if (this.isScanning != value)
                {
                    this.PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(IsScanningPropertyName));
                    this.isScanning = value;
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(IsScanningPropertyName));
                }
            }
        }

        public string Folder
        {
            get => this.folder;
            set
            {
                if (this.folder != value)
                {
                    this.PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(FolderPropertyName));
                    this.folder = value;
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(FolderPropertyName));
                }
            }
        }

        public SortKey SortKey
        {
            get
            {
                return this.sortKey;
            }

            set
            {
                if (this.sortKey != value)
                {
                    this.PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(SortKeyPropertyName));
                    this.sortKey = value;
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(SortKeyPropertyName));
                }
            }
        }

        public IReadOnlyList<FileSystemEntry> TopLevelFolders
        {
            get => this.topLevelFolders;
            set
            {
                if (this.topLevelFolders != value)
                {
                    this.PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(TopLevelFoldersPropertyName));
                    this.topLevelFolders = value;
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(TopLevelFoldersPropertyName));
                }
            }
        }

        public IReadOnlyList<FileSystemEntry> Children
        {
            get
            {
                return this.children;
            }

            set
            {
                if (this.children != value)
                {
                    this.PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(ChildrenPropertyName));
                    this.children = value;
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(ChildrenPropertyName));
                }
            }
        }

        public FileSystemEntry Selected
        {
            get
            {
                return this.selected;
            }

            set
            {
                if (this.selected != value)
                {
                    this.PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(SelectedPropertyName));
                    this.selected = value;
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(SelectedPropertyName));
                }
            }
        }
    }
}
