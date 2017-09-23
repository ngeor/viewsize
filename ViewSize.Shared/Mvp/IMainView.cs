using CRLFLabs.ViewSize.Drawing;
using CRLFLabs.ViewSize.TreeMap;
using System;
using System.Collections.Generic;

namespace CRLFLabs.ViewSize.Mvp
{
    /// <summary>
    /// Main view.
    /// </summary>
    public interface IMainView<T>
        where T : class, IFileSystemEntry<T>, new()
    {
        string SelectedFolder { get; }
        SizeD TreeMapActualSize { get; }

        void EnableUI(bool enable);
        void RunOnGuiThread(Action action);

        void ShowError(string message);
        void ShowError(Exception ex);

        void SetFolders(IList<T> topLevelFolders);
        void SetTreeMapDataSource(TreeMapDataSource<T> treeMapDataSource);
        void SetDurationLabel(string durationLabel);
        void SetSelectedTreeViewItem(string path);
    }
}
