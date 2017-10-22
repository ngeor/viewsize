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

        private void SetupFolderChooserViewModel()
        {
            txtFolder.Changed += TxtFolder_Changed;
            txtFolder.StringValue = Model.Folder;
            Model.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == MainModel.FolderPropertyName)
                {
                    txtFolder.StringValue = Model.Folder;
                }
            };
        }

        void TxtFolder_Changed(object sender, EventArgs e)
        {
            Model.Folder = txtFolder.StringValue;
        }
    }
}
