using CRLFLabs.ViewSize.Drawing;
using CRLFLabs.ViewSize.TreeMap;
using System;
using CRLFLabs.ViewSize.IO;

namespace CRLFLabs.ViewSize.Mvp
{
    /// <summary>
    /// Main view.
    /// </summary>
    public interface IMainView : IView
    {
        /// <summary>
        /// Occurs when the user wants to start the folder scan.
        /// </summary>
        event EventHandler OnBeginScanClick;

        /// <summary>
        /// Occurs when the user wants to abort the folder scan.
        /// </summary>
        event EventHandler OnCancelScanClick;

        /// <summary>
        /// Occurs when the user has selected a file system entry on the tree view.
        /// </summary>
        event EventHandler<FileSystemEventArgs> OnTreeViewSelectionChanged;

        string SelectedFolder { get; }
        SizeD TreeMapActualSize { get; }

        void EnableUI(bool enable);

        void SetTreeMapDataSource(TreeMapDataSource treeMapDataSource);
        void SetDurationLabel(string durationLabel);
        void SetSelectedTreeViewItem(FileSystemEntry selectedFileSystemEntry);
        void SetScanningItem(string path);
    }
}
