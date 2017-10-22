using System;
using System.ComponentModel;
using CRLFLabs.ViewSize.IO;
using System.Collections.Generic;

namespace CRLFLabs.ViewSize.Mvp
{
    public interface IMainModel : INotifyPropertyChanging, INotifyPropertyChanged
    {
        string Folder { get; set; }
        SortKey SortKey { get; set; }
        IReadOnlyList<FileSystemEntry> Children { get; set; }
        FileSystemEntry Selected { get; set; }
    }
}
