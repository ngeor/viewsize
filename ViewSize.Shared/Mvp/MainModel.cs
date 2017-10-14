using System;
using System.ComponentModel;
using CRLFLabs.ViewSize.IO;
using CRLFLabs.ViewSize.TreeMap;

namespace CRLFLabs.ViewSize.Mvp
{
    public class MainModel : IMainModel
    {
        public const string SortKeyProperty = "SortKey";
        public const string DataSourceProperty = "DataSource";

        private SortKey _sortKey;
        private TreeMapDataSource _dataSource;

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
    }
}
