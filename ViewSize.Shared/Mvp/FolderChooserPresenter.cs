using System;
using System.ComponentModel;
using CRLFLabs.ViewSize.Settings;

namespace CRLFLabs.ViewSize.Mvp
{
    /// <summary>
    /// Folder chooser presenter.
    /// </summary>
    public class FolderChooserPresenter : PresenterBase<IFolderChooserView, IMainModel>
    {
        public FolderChooserPresenter(IFolderChooserView view, IMainModel model, ISettingsManager settingsManager, ICommandBus commandBus)
            : base(view, model)
        {
            SettingsManager = settingsManager;
            Model.Folder = settingsManager.Settings.SelectedFolder;
            Model.PropertyChanged += Model_PropertyChanged;
            commandBus.Subscribe("SelectFolder", () => View_OnSelectFolderClick(null, EventArgs.Empty));
        }

        private ISettingsManager SettingsManager { get; }

        protected override void OnViewLoad(object sender, EventArgs e)
        {
            base.OnViewLoad(sender, e);
            View.OnSelectFolderClick += View_OnSelectFolderClick;
            View.Folder = Model.Folder;
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
            if (e.PropertyName == MainModel.FolderPropertyName)
            {
                View.Folder = Model.Folder;
                SettingsManager.Settings.SelectedFolder = Model.Folder;
            }
        }
    }
}
