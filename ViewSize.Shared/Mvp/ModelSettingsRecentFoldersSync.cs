// <copyright file="ModelSettingsRecentFoldersSync.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System.ComponentModel;
using System.Linq;
using CRLFLabs.ViewSize.Settings;

namespace CRLFLabs.ViewSize.Mvp
{
    /// <summary>
    /// Synchronizes model with settings storage.
    /// This is only needed on Windows; Mac has its own recent folders mechanism.
    /// </summary>
    public class ModelSettingsRecentFoldersSync : MainModelSettingsObserver
    {
        public ModelSettingsRecentFoldersSync(IMainModel model, ISettingsManager settingsManager)
            : base(model, settingsManager)
        {
            Model.RecentFolders = SettingsManager.Settings.RecentFolders ?? new string[0];
            Model.PropertyChanged += Model_PropertyChanged;
        }

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case MainModel.RecentFoldersPropertyName:
                    SettingsManager.Settings.RecentFolders = Model.RecentFolders.ToArray();
                    break;
                default:
                    break;
            }
        }
    }
}
