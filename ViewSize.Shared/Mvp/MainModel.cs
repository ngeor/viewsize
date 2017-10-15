using System;
using System.ComponentModel;
using CRLFLabs.ViewSize.IO;
using CRLFLabs.ViewSize.TreeMap;
using System.Collections.Generic;

namespace CRLFLabs.ViewSize.Mvp
{
    public class MainModel : IMainModel
    {
        public const string SortKeyProperty = "SortKey";
        public const string DataSourceProperty = "DataSource";
        public const string TopLevelFoldersProperty = "TopLevelFolders";

        private SortKey _sortKey;
        private TreeMapDataSource _dataSource;
        private IReadOnlyList<FileSystemEntry> _topLevelFolders;

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
                    PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(SortKeyProperty));
                    _sortKey = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(SortKeyProperty));
                }
            }
        }

        public TreeMapDataSource DataSource
        {
            get
            {
                return _dataSource;
            }
            set
            {
                if (_dataSource != value)
                {
                    PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(DataSourceProperty));
                    _dataSource = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs(DataSourceProperty));
                }
            }
        }

        public IReadOnlyList<FileSystemEntry> TopLevelFolders
        {
            get
            {
                return _topLevelFolders;
            }
            set
            {
                if (_topLevelFolders != value)
                {
                    PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(TopLevelFoldersProperty));
                    _topLevelFolders = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(TopLevelFoldersProperty));
                }
            }
        }
    }
}
