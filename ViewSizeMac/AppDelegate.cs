using System;
using AppKit;
using CRLFLabs.ViewSize.Mvp;
using Foundation;
using CRLFLabs.ViewSize.Settings;
using System.Linq;

namespace ViewSizeMac
{
    [Register("AppDelegate")]
    public partial class AppDelegate : NSApplicationDelegate, IApplicationView
    {
        public AppDelegate()
        {
            new ApplicationPresenter(this, SettingsManager.Instance);
        }

        public event EventHandler Closing;

        public override void DidFinishLaunching(NSNotification notification)
        {
            // Insert code here to initialize your application
            mnuFileSizeTreeMap.State = NSCellStateValue.On;
        }

        public override void WillTerminate(NSNotification notification)
        {
            // Insert code here to tear down your application
        }

        public override NSApplicationTerminateReply ApplicationShouldTerminate(NSApplication sender)
        {
            Closing?.Invoke(this, EventArgs.Empty);
            return NSApplicationTerminateReply.Now;
        }

        [Export("openDocument:")]
        void OnOpenDocument(NSObject sender)
        {
            FindViewController().TriggerSelectFolderClick();
            ShowMainWindow();
        }

        [Export("fileSizeTreeMap:")]
        void OnFileSizeTreeMap(NSObject sender)
        {
            mnuFileSizeTreeMap.State = NSCellStateValue.On;
            mnuFileCountTreeMap.State = NSCellStateValue.Off;
            var model = Registry.Instance.Get<IMainModel>();
            model.SortKey = CRLFLabs.ViewSize.IO.SortKey.Size;
        }

        [Export("fileCountTreeMap:")]
        void OnFileCountTreeMap(NSObject sender)
        {
            mnuFileCountTreeMap.State = NSCellStateValue.On;
            mnuFileSizeTreeMap.State = NSCellStateValue.Off;
            var model = Registry.Instance.Get<IMainModel>();
            model.SortKey = CRLFLabs.ViewSize.IO.SortKey.Count;
        }

        /// <summary>
        /// Opens the file from the recently opened files list.
        /// </summary>
        /// <returns><c>true</c>, if file was opened, <c>false</c> otherwise.</returns>
        /// <param name="sender">Sender.</param>
        /// <param name="filename">Filename.</param>
        public override bool OpenFile(NSApplication sender, string filename)
        {
            var model = Registry.Instance.Get<IFolderChooserModel>();
            model.Folder = filename;
            ShowMainWindow();
            return true;
        }

        private ViewController FindViewController()
        {
            var q =
                from w in NSApplication.SharedApplication.Windows
                select w.ContentViewController as ViewController;
            return q.FirstOrDefault();
        }

        private void ShowMainWindow()
        {
            var q =
                from w in NSApplication.SharedApplication.Windows
                where w.ContentViewController is ViewController
                select w;
            var window = q.FirstOrDefault();
            window.MakeKeyAndOrderFront(this);
        }
    }
}
