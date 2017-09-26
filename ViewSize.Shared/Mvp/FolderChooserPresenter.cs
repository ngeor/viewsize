using System;
using System.ComponentModel;
using CRLFLabs.ViewSize.Settings;

namespace CRLFLabs.ViewSize.Mvp
{
    /// <summary>
    /// Folder chooser presenter.
    /// </summary>
    public class FolderChooserPresenter : PresenterBase<IFolderChooserView, IFolderChooserModel>
    {
        public FolderChooserPresenter(IFolderChooserView view, IFolderChooserModel model, ISettingsManager settingsManager)
            : base(view, model)
        {
            SettingsManager = settingsManager;

            // cannot use AttachToModel because we need SettingsManager here
            Model.Folder = SettingsManager.Settings.SelectedFolder;
            Model.PropertyChanged += Model_PropertyChanged;
        }

        private ISettingsManager SettingsManager { get; }

        protected override void AttachToView()
        {
            View.OnSelectFolderClick += View_OnSelectFolderClick;
        }

        private void View_OnSelectFolderClick(object sender, EventArgs e)
        {
            string folder = View.SelectFolder();
            if (folder != null)
            {
                Model.Folder = folder;
            }
        }

        void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SettingsManager.Settings.SelectedFolder = Model.Folder;
        }
    }
}
