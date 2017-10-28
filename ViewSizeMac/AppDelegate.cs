using System;
using AppKit;
using CRLFLabs.ViewSize.Mvp;
using Foundation;
using System.Linq;

namespace ViewSizeMac
{
    [Register("AppDelegate")]
    [Presenter(typeof(ApplicationPresenter))]
    [Presenter(typeof(MenuPresenter))]
    public partial class AppDelegate : NSApplicationDelegate, IApplicationView, IMenuView
    {
        public event EventHandler Load;
        public event EventHandler Closing;
        public event EventHandler FileSizeTreeMapClick;
        public event EventHandler FileCountTreeMapClick;
        public event EventHandler FileOpenClick;

        public bool IsFileSizeTreeMapChecked
        {
            get => mnuFileSizeTreeMap.State == NSCellStateValue.On;
            set => mnuFileSizeTreeMap.State = value ? NSCellStateValue.On : NSCellStateValue.Off;
        }

        public bool IsFileCountTreeMapChecked
        {
            get => mnuFileCountTreeMap.State == NSCellStateValue.On;
            set => mnuFileCountTreeMap.State = value ? NSCellStateValue.On : NSCellStateValue.Off;
        }

        public IMainModel Model { get; set; }

        public override void DidFinishLaunching(NSNotification notification)
        {
            // Insert code here to initialize your application
            PresenterFactory.Create(this);
            Load?.Invoke(this, EventArgs.Empty);
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
            FileOpenClick?.Invoke(this, EventArgs.Empty);
        }

        [Export("fileSizeTreeMap:")]
        void OnFileSizeTreeMap(NSObject sender)
        {
            FileSizeTreeMapClick.Invoke(this, EventArgs.Empty);
        }

        [Export("fileCountTreeMap:")]
        void OnFileCountTreeMap(NSObject sender)
        {
            FileCountTreeMapClick.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Opens the file from the recently opened files list.
        /// </summary>
        /// <returns><c>true</c>, if file was opened, <c>false</c> otherwise.</returns>
        /// <param name="sender">Sender.</param>
        /// <param name="filename">Filename.</param>
        public override bool OpenFile(NSApplication sender, string filename)
        {
            Model.Folder = filename;
            ShowMainWindow();
            return true;
        }

        public void ShowMainWindow()
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
