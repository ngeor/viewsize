using System;

namespace CRLFLabs.ViewSize.Mvp
{
    /// <summary>
    /// Folder chooser presenter.
    /// </summary>
    public class FolderChooserPresenter : IFolderChooserPresenter
    {
        private IFolderChooserView View { get; }
        private IFolderChooserModel Model { get; }

        public FolderChooserPresenter(IFolderChooserView view, IFolderChooserModel model)
        {
            View = view;
            Model = model;
        }

        public void OnSelectFolder()
        {
            string folder = View.SelectFolder();
            if (folder != null)
            {
                Model.Folder = folder;
            }
        }
    }
}
