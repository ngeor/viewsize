// <copyright file="ModelSettingsFolderSync.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System.ComponentModel;
using CRLFLabs.ViewSize.Settings;

namespace CRLFLabs.ViewSize.Mvp
{
    /// <summary>
    /// Synchronizes model with settings storage.
    /// </summary>
    public class ModelSettingsFolderSync : MainModelSettingsObserver
    {
        public ModelSettingsFolderSync(IMainModel model, ISettingsManager settingsManager)
            : base(model, settingsManager)
        {
            Model.Folder = SettingsManager.Settings.SelectedFolder;
            Model.PropertyChanged += Model_PropertyChanged;
        }

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case MainModel.FolderPropertyName:
                    SettingsManager.Settings.SelectedFolder = Model.Folder;
                    break;
                default:
                    break;
            }
        }
    }
}
