using System;
using System.ComponentModel;
using CRLFLabs.ViewSize.IO;
using CRLFLabs.ViewSize.TreeMap;
using System.Collections.Generic;

namespace CRLFLabs.ViewSize.Mvp
{
    public interface IMainModel : INotifyPropertyChanging, INotifyPropertyChanged
    {
        SortKey SortKey { get; set; }
        TreeMapDataSource DataSource { get; set; }
        IReadOnlyList<FileSystemEntry> TopLevelFolders { get; set; }
    }
}
