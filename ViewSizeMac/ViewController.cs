using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppKit;
using CRLFLabs.ViewSize;
using Foundation;
using CRLFLabs.ViewSize.Drawing;
using CRLFLabs.ViewSize.TreeMap;
using CRLFLabs.ViewSize.Mvp;

namespace ViewSizeMac
{
    public partial class ViewController : NSViewController, IMainView, IFolderChooserView, IFolderChooserModel
    {
        private MainPresenter _mainPresenter;
        private FolderChooserPresenter _folderChooserPresenter;

        public ViewController(IntPtr handle) : base(handle)
        {
            TroubleshootNativeCrashes();
        }

        private static void TroubleshootNativeCrashes()
        {
#if DEBUG
            new System.Threading.Thread(() =>
            {
                while (true)
                {
                    System.Threading.Thread.Sleep(1000);
                    GC.Collect();
                }
            }).Start();
#endif
        }

        /// <summary>
        /// Called after the view has loaded.
        /// Do any additional setup after loading the view.
        /// </summary>
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            CreatePresenters();

            // create and assign the outline delegate.
            // since this does not depend on the datasource,
            // it can be done upfront. Also subscribing to the event only once.
            var folderOutlineDelegate = new FolderOutlineDelegate();
            folderOutlineDelegate.SelectionChanged += (sender, e) =>
            {
                var m = outlineView.ItemAtRow(outlineView.SelectedRow) as FileSystemEntry;
                _mainPresenter.OnTreeViewSelectionChanged(m);
            };
            outlineView.Delegate = folderOutlineDelegate;
        }

        private void CreatePresenters()
        {
            _mainPresenter = new MainPresenter(this);
            _folderChooserPresenter = new FolderChooserPresenter(this, this);
        }

        public override NSObject RepresentedObject
        {
            get
            {
                return base.RepresentedObject;
            }
            set
            {
                base.RepresentedObject = value;
                // Update the view, if already loaded.
            }
        }

        #region Cocoa Actions

        partial void OnSelectFolder(NSObject sender) => _folderChooserPresenter.OnSelectFolder();
        partial void OnScan(NSObject sender) => _mainPresenter.OnBeginScan();
        partial void OnCancelScan(NSObject sender) => _mainPresenter.OnCancelScan();

        #endregion

        #region IMainView

        public SizeD TreeMapActualSize => folderGraph.ActualSize.Size;
        string IMainView.SelectedFolder => txtFolder.StringValue;

        public void EnableUI(bool enable)
        {
            btnScan.Enabled = enable;
            btnCancel.Enabled = !enable;
            btnSelectFolder.Enabled = enable;
            txtFolder.Enabled = enable;
        }

        public void RunOnGuiThread(Action action) => InvokeOnMainThread(action);

        public void ShowError(Exception ex) => ShowExceptionAlert(ex);

        public void ShowError(string message) => MessageBox.ShowMessage(message);

        public void SetFolders(IList<FileSystemEntry> topLevelFolders)
        {
            var dataSource = new FolderOutlineDataSource(topLevelFolders);
            outlineView.DataSource = dataSource;
        }

        public void SetTreeMapDataSource(TreeMapDataSource treeMapDataSource) => folderGraph.DataSource = treeMapDataSource;

        public void SetDurationLabel(string durationLabel) => lblStatus.StringValue = durationLabel;

        public void SetSelectedTreeViewItem(FileSystemEntry selection)
        {
            FolderOutlineDataSource dataSource = outlineView.DataSource as FolderOutlineDataSource;
            if (dataSource == null)
            {
                return;
            }

            if (selection == null)
            {
                outlineView.DeselectAll(this);
                return;
            }

            foreach (var ancestor in selection.Ancestors())
            {
                outlineView.ExpandItem(ancestor, false);
            }

            var row = outlineView.RowForItem(selection);
            outlineView.SelectRow(row, false);
            outlineView.ScrollRowToVisible(row);
        }

        #endregion

        #region IFolderChooserView

        public string SelectFolder()
        {
            var dlg = NSOpenPanel.OpenPanel;
            dlg.CanChooseFiles = false;
            dlg.CanChooseDirectories = true;
            dlg.CanCreateDirectories = false;
            if (dlg.RunModal() == 1)
            {
                return dlg.Url.Path;
            }

            return null;
        }

        #endregion

        #region IFolderChooserModel

        string IFolderChooserModel.Folder
        {
            get
            {
                return txtFolder.StringValue;
            }
            set
            {
                txtFolder.StringValue = value;
            }
        }

        #endregion

        private void ReportProgress(object sender, FileSystemEventArgs args)
        {
            // TODO: this is too slow for every file and folder there is.
            //InvokeOnMainThread(() =>
            //{
            //    lblStatus.StringValue = args.Folder.Path;
            //});
        }

        private void ShowExceptionAlert(Exception ex)
        {
            MessageBox.ShowMessage(ex.Message + ex.StackTrace, ex.Message);
        }
    }
}
