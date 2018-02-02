// <copyright file="IMenuView.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace CRLFLabs.ViewSize.Mvp
{
    public interface IMenuView : IView<IMainModel>
    {
        event EventHandler FileSizeTreeMapClick;

        event EventHandler FileCountTreeMapClick;

        event EventHandler FileOpenClick;

        event EventHandler<RecentFileEventArgs> OpenRecentFileClick;

        bool IsFileSizeTreeMapChecked { get; set; }

        bool IsFileCountTreeMapChecked { get; set; }

        void ShowMainWindow();

        void SetRecentFolders(IEnumerable<string> folders);
    }
}
