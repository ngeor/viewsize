using System;
using System.Collections.Generic;
using System.ComponentModel;
using CRLFLabs.ViewSize.Settings;

namespace CRLFLabs.ViewSize.Mvp
{
    /// <summary>
    /// Folder chooser presenter.
    /// </summary>
    public class FolderChooserPresenter : IFolderChooserPresenter
    {
        private IFolderChooserView View { get; }
        private IFolderChooserModel Model { get; }
        private ISettingsManager SettingsManager { get; }

        public FolderChooserPresenter(IFolderChooserView view, IFolderChooserModel model, ISettingsManager settingsManager)
        {
            View = view;
            Model = model;
            SettingsManager = settingsManager;

            Model.Folder = SettingsManager.Settings.SelectedFolder;
            Model.PropertyChanged += Model_PropertyChanged;
        }


        public void OnSelectFolder()
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
