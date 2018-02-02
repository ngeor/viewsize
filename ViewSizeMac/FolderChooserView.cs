// <copyright file="FolderChooserView.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;
using AppKit;
using CRLFLabs.ViewSize.Mvp;
using Foundation;

namespace ViewSizeMac
{
    [Presenter(typeof(FolderChooserPresenter))]
    partial class ViewController : IFolderChooserView
    {
        public event EventHandler OnSelectFolderClick;

        string IFolderChooserView.Folder
        {
            get => txtFolder.StringValue;
            set => txtFolder.StringValue = value;
        }

        public bool HasNativeRecentFolders => true;

        public string SelectFolder()
        {
            var dlg = NSOpenPanel.OpenPanel;
            dlg.CanChooseFiles = false;
            dlg.CanChooseDirectories = true;
            dlg.CanCreateDirectories = false;
            if (dlg.RunModal() == 1)
            {
                // TODO move this to the presenter
                NSDocumentController.SharedDocumentController.NoteNewRecentDocumentURL(dlg.Url);
                return dlg.Url.Path;
            }

            return null;
        }

        partial void OnSelectFolder(NSObject sender) => OnSelectFolderClick?.Invoke(this, EventArgs.Empty);

        private void SetupFolderChooserView()
        {
            txtFolder.Changed += TxtFolder_Changed;
        }

        void TxtFolder_Changed(object sender, EventArgs e)
        {
            Model.Folder = txtFolder.StringValue;
        }
    }
}
