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
        public FolderChooserPresenter(IFolderChooserView view, ISettingsManager settingsManager)
            : base(view)
        {
            SettingsManager = settingsManager;
        }

        private ISettingsManager SettingsManager { get; }

        protected override void OnViewLoad(object sender, EventArgs e)
        {
            base.OnViewLoad(sender, e);
            View.OnSelectFolderClick += View_OnSelectFolderClick;
        }

        protected override IFolderChooserModel CreateModel()
        {
            var result = new FolderChooserModel
            {
                Folder = SettingsManager.Settings.SelectedFolder
            };

            result.PropertyChanged += Model_PropertyChanged;
            return result;
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
