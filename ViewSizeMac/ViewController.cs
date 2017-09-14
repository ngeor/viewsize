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
    public partial class ViewController : NSViewController, IMainView
    {
        private readonly MainPresenter _mainPresenter = new MainPresenter();
        public event EventHandler SelectFolderClick;
        public event EventHandler ScanClick;
        public event EventHandler CancelClick;

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

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Do any additional setup after loading the view.
            _mainPresenter.View = this;

            // create and assign the outline delegate.
            // since this does not depend on the datasource,
            // it can be done upfront. Also subscribing to the event only once.
            var folderOutlineDelegate = new FolderOutlineDelegate();
            folderOutlineDelegate.SelectionChanged += (sender, e) =>
            {
                FSEntryModel m = outlineView.ItemAtRow(outlineView.SelectedRow) as FSEntryModel;
                //folderGraph.Selected = treeMapDataSource.Find(m.Path);
            };
            outlineView.Delegate = folderOutlineDelegate;
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

        public string SelectedFolder
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

        public SizeD TreeMapActualSize => folderGraph.ActualSize.Size;

        partial void OnSelectFolder(NSObject sender)
        {
            SelectFolderClick(this, EventArgs.Empty);
        }

        partial void OnScan(NSObject sender)
        {
            ScanClick(this, EventArgs.Empty);
        }

        partial void OnCancelScan(NSObject sender)
        {
            CancelClick(this, EventArgs.Empty);
        }

        private void ReportProgress(object sender, FileSystemEventArgs args)
        {
            // TODO: this is too slow for every file and folder there is.
            //InvokeOnMainThread(() =>
            //{
            //    lblStatus.StringValue = args.Folder.Path;
            //});
        }

        /// <summary>
        /// Invokes the given action on the main thread, using the specified argument.
        /// </summary>
        /// <param name="action">The action to invoke.</param>
        /// <param name="argument">The argument to pass to the action.</param>
        /// <typeparam name="TArg">The type parameter of the argument.</typeparam>
        private void InvokeOnMainThread<TArg>(Action<TArg> action, TArg argument)
        {
            InvokeOnMainThread(() =>
            {
                action(argument);
            });
        }

        private void ShowExceptionAlert(Exception ex)
        {
            MessageBox.ShowMessage(ex.Message + ex.StackTrace, ex.Message);
        }

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

        public void EnableUI(bool enable)
        {
            btnScan.Enabled = enable;
            btnCancel.Enabled = !enable;
            btnSelectFolder.Enabled = enable;
            txtFolder.Enabled = enable;
        }

        public void RunOnGuiThread(Action action) => InvokeOnMainThread(action);

        public void ShowError(Exception ex) => ShowExceptionAlert(ex);

        public void SetResult(FolderScanner folderScanner, TreeMapDataSource treeMapDataSource, string durationLabel)
        {
            var models = FSEntryModel.ToModels(folderScanner.TopLevelFolders);
            var dataSource = new FolderOutlineDataSource(models);
            outlineView.DataSource = dataSource;

            folderGraph.DataSource = treeMapDataSource;
            lblStatus.StringValue = durationLabel;
        }
    }
}
