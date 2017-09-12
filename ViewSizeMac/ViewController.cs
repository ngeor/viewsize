using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppKit;
using CRLFLabs.ViewSize;
using Foundation;
using CRLFLabs.ViewSize.Drawing;
using CRLFLabs.ViewSize.TreeMap;

namespace ViewSizeMac
{
    public partial class ViewController : NSViewController
    {
        private readonly FolderScanner folderScanner = new FolderScanner();
        private readonly Renderer renderer = new Renderer();
        private TreeMapDataSource treeMapDataSource;

        public ViewController(IntPtr handle) : base(handle)
        {
            folderScanner.Scanning += ReportProgress;
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

        partial void OnSelectFolder(NSObject sender)
        {
            var dlg = NSOpenPanel.OpenPanel;
            dlg.CanChooseFiles = false;
            dlg.CanChooseDirectories = true;
            dlg.CanCreateDirectories = false;
            if (dlg.RunModal() == 1)
            {
                txtFolder.StringValue = dlg.Url.Path;
            }
        }

        partial void OnScan(NSObject sender)
        {
            EnableUI(false);
            string path = txtFolder.StringValue;
            RectangleD bounds = folderGraph.Bounds.ToRectangleD();
            Task.Run(() =>
            {
                try
                {
                    ScanInBackgroundThread(path, bounds);
                    InvokeOnMainThread(UpdateViewsOnMainThread);
                }
                catch (Exception ex)
                {
                    InvokeOnMainThread(ShowExceptionAlert, ex);
                }
                finally
                {
                    InvokeOnMainThread(() =>
                    {
                        EnableUI(true);
                    });
                }
            });
        }

        private void ScanInBackgroundThread(string path, RectangleD bounds)
        {
            folderScanner.Scan(path);
            treeMapDataSource = renderer.Render(bounds, folderScanner.TopLevelFolders);
        }

        private void UpdateViewsOnMainThread()
        {
            var models = FSEntryModel.ToModels(folderScanner.TopLevelFolders);
            var dataSource = new FolderOutlineDataSource(models);
            outlineView.DataSource = dataSource;
            outlineView.Delegate = new FolderOutlineDelegate();
            folderGraph.DataSource = treeMapDataSource;
        }

        partial void OnCancelScan(NSObject sender)
        {
            folderScanner.Cancel();
        }

        private void ReportProgress(object sender, FileSystemEventArgs args)
        {
            // TODO: this is too slow for every file and folder there is.
            //InvokeOnMainThread(() =>
            //{
            //    lblStatus.StringValue = args.Folder.Path;
            //});
        }

        private void EnableUI(bool enable)
        {
            btnScan.Enabled = enable;
            btnCancel.Enabled = !enable;
            btnSelectFolder.Enabled = enable;
            txtFolder.Enabled = enable;
            lblStatus.StringValue = $"Finished in {folderScanner.Duration}";
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
    }
}
