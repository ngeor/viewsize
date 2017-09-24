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

            SettingsManager settingsManager = new SettingsManager();
            Settings settings = settingsManager.Load();
            Model.Folder = settings.SelectedFolder;
        }

        public void OnSelectFolder()
        {
            string folder = View.SelectFolder();
            if (folder != null)
            {
                Model.Folder = folder;
            }
        }

        public void SaveSettings()
        {
            // TODO : new is glue
            // TODO : each presenter now will save settings one by one, which is bad
            SettingsManager settingsManager = new SettingsManager();
            Settings settings = settingsManager.Load();
            settings.SelectedFolder = Model.Folder;
            settingsManager.Save(settings);
        }
    }
}
