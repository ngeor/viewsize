using System;
using AppKit;
using CRLFLabs.ViewSize.Mvp;
using Foundation;
using CRLFLabs.ViewSize.Settings;

namespace ViewSizeMac
{
    [Register("AppDelegate")]
    public class AppDelegate : NSApplicationDelegate, IApplicationView
    {
        public AppDelegate()
        {
            new ApplicationPresenter(this, SettingsManager.Instance);
        }

        public event EventHandler Closing;

        public override void DidFinishLaunching(NSNotification notification)
        {
            // Insert code here to initialize your application
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
    }
}
