// <copyright file="IMainModel.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.ComponentModel;
using CRLFLabs.ViewSize.IO;

namespace CRLFLabs.ViewSize.Mvp
{
    public interface IMainModel : INotifyPropertyChanging, INotifyPropertyChanged
    {
        bool IsScanning { get; set; }

        string Folder { get; set; }

        SortKey SortKey { get; set; }

        IReadOnlyList<FileSystemEntry> Children { get; set; }

        /// <summary>
        /// Gets or sets the top level folders.
        /// </summary>
        /// <value>The top level folders.</value>
        /// <remarks>
        /// Setting this will cause the TreeMapPresenter to render.
        /// </remarks>
        IReadOnlyList<FileSystemEntry> TopLevelFolders { get; set; }

        FileSystemEntry Selected { get; set; }

        IReadOnlyList<string> RecentFolders { get; set; }
    }
}
