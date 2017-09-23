using CRLFLabs.ViewSize.Drawing;
using CRLFLabs.ViewSize.TreeMap;
using System;
using System.Collections.Generic;

namespace CRLFLabs.ViewSize.Mvp
{
    /// <summary>
    /// Main view.
    /// </summary>
    public interface IMainView
    {
        string SelectedFolder { get; }
        SizeD TreeMapActualSize { get; }

        void EnableUI(bool enable);
        void RunOnGuiThread(Action action);

        void ShowError(string message);
        void ShowError(Exception ex);

        void SetFolders(IReadOnlyList<FileSystemEntry> topLevelFolders);
        void SetTreeMapDataSource(TreeMapDataSource treeMapDataSource);
        void SetDurationLabel(string durationLabel);
        void SetSelectedTreeViewItem(FileSystemEntry selectedFileSystemEntry);
    }
}
