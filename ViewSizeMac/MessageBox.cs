using System;
using AppKit;

namespace ViewSizeMac
{
    /// <summary>
    /// A utility class to show messages using NSAlert.
    /// </summary>
    public static class MessageBox
    {
        public static void ShowMessage(string message, string caption = null, NSAlertStyle alertStyle = NSAlertStyle.Critical)
        {
            NSAlert alert = new NSAlert
            {
                AlertStyle = alertStyle,
                MessageText = caption ?? message,
                InformativeText = message
            };

            alert.RunModal();
        }
    }
}
