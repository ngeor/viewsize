using CRLFLabs.ViewSize.Drawing;
using CRLFLabs.ViewSize.TreeMap;
using System;

namespace CRLFLabs.ViewSize.Mvp
{
    public interface IMainView
    {
        string SelectedFolder { get; set; }
        event EventHandler SelectFolderClick;
        string SelectFolder();

        event EventHandler ScanClick;
        event EventHandler CancelClick;

        void EnableUI(bool enable);

        SizeD TreeMapActualSize { get; }
        void RunOnGuiThread(Action action);
        void ShowError(Exception ex);
        void SetResult(FolderScanner folderScanner, TreeMapDataSource treeMapDataSource, string durationLabel);
    }
}
