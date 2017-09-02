using System;
using System.Threading.Tasks;
using AppKit;
using CRLFLabs.ViewSize;
using Foundation;

namespace ViewSizeMac
{
    public partial class ViewController : NSViewController
    {
        public ViewController(IntPtr handle) : base(handle)
        {
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
                    FileEntry root = new FileEntry(path);
                    root.Calculate(ReportProgress, (finished) => { });
                    InvokeOnMainThread(() => {
                        outlineView.DataSource = new FolderOutlineDataSource(new FolderViewModel(root));
                    });
                }
                catch (Exception ex)
                {
                    InvokeOnMainThread(() => {
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

        private void ReportProgress(int number)
        {
            InvokeOnMainThread(() => {
            });
        }

        private void EnableUI(bool enable)
        {
			btnScan.Enabled = enable;
			btnSelectFolder.Enabled = enable;
			txtFolder.Enabled = enable;
        }
    }
}
