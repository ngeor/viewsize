// <copyright file="IMainView.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;
using CRLFLabs.ViewSize.IO;

namespace CRLFLabs.ViewSize.Mvp
{
    /// <summary>
    /// Main view.
    /// </summary>
    public interface IMainView : IView<IMainModel>
    {
        /// <summary>
        /// Occurs when the user wants to start the folder scan.
        /// </summary>
        event EventHandler BeginScanClick;

        /// <summary>
        /// Occurs when the user wants to abort the folder scan.
        /// </summary>
        event EventHandler CancelScanClick;

        /// <summary>
        /// Occurs when the user has selected a file system entry on the tree view.
        /// </summary>
        event EventHandler<FileSystemEventArgs> TreeViewSelectionChanged;

        event EventHandler UpOneLevelClick;

        event EventHandler<CanExecuteEventArgs> UpOneLevelCanExecute;

        void EnableUI(bool enable);

        void SetTreeViewContents();

        void SetDurationLabel(string durationLabel);

        void SetSelectedTreeViewItem(FileSystemEntry selectedFileSystemEntry);

        void SetScanningItem(string path);

        void RunOnGuiThread(Action action);

        void ShowError(string message);

        void ShowError(Exception ex);
    }
}
