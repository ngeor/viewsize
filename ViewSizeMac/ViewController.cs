using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppKit;
using CRLFLabs.ViewSize;
using Foundation;

namespace ViewSizeMac
{
    public partial class ViewController : NSViewController
    {
        private readonly FolderScanner folderScanner = new FolderScanner();

        public ViewController(IntPtr handle) : base(handle)
        {
            folderScanner.Scanning += ReportProgress;
            TroubleshootNativeCrashes();
        }

        private static void TroubleshootNativeCrashes()
        {
            new System.Threading.Thread(() =>
            {
                while (true)
                {
                    System.Threading.Thread.Sleep(1000);
                    GC.Collect();
                }
            }).Start();
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
            Task.Run(() =>
            {
                try
                {
                    folderScanner.Scan(path);
                    InvokeOnMainThread(() =>
                    {
                        var models = FSEntryModel.ToModels(folderScanner.TopLevelFolders);
                        var dataSource = new FolderOutlineDataSource(models);
                        outlineView.DataSource = dataSource;
                        outlineView.Delegate = new FolderOutlineDelegate();
                        folderGraph.DataSource = models;
                    });
                }
                catch (Exception ex)
                {
                    InvokeOnMainThread(() =>
                    {
                        NSAlert alert = new NSAlert
                        {
                            AlertStyle = NSAlertStyle.Critical,
                            MessageText = ex.Message,
                            InformativeText = ex.Message
                        };

                        alert.RunModal();
                    });
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
    }
}
