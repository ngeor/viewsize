using CRLFLabs.ViewSize.Drawing;
using CRLFLabs.ViewSize.TreeMap;
using System;
using System.Collections.Generic;

namespace CRLFLabs.ViewSize.Mvp
{
    /// <summary>
    /// Main view.
    /// </summary>
    public interface IMainView : IView
    {
        string SelectedFolder { get; }
        SizeD TreeMapActualSize { get; }

        void EnableUI(bool enable);

        void SetTreeMapDataSource(TreeMapDataSource treeMapDataSource);
        void SetDurationLabel(string durationLabel);
        void SetSelectedTreeViewItem(FileSystemEntry selectedFileSystemEntry);
        void SetScanningItem(string path);
    }
}
