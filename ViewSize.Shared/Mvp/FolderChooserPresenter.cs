// <copyright file="FolderChooserPresenter.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;
using System.Linq;
using CRLFLabs.ViewSize.Settings;
using System.ComponentModel;

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

            // allows MenuPresenter to trigger a SelectFolder request
            commandBus.Subscribe("SelectFolder", () => View_OnSelectFolderClick(null, EventArgs.Empty));
        }

        private ISettingsManager SettingsManager { get; }

        protected override void OnViewLoad(object sender, EventArgs e)
        {
            base.OnViewLoad(sender, e);
            InitModelFromSettings();
            SubscribeToViewEvents();
            InitViewFromModel();
        }

        private void InitModelFromSettings()
        {
            Model.Folder = SettingsManager.Settings.SelectedFolder;
            Model.PropertyChanged += Model_PropertyChanged;
        }

        private void InitViewFromModel()
        {
            View.Folder = Model.Folder;
        }

        private void SubscribeToViewEvents()
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

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == MainModel.FolderPropertyName)
            {
                var folder = Model.Folder;
                View.Folder = folder;
                SettingsManager.Settings.SelectedFolder = folder;
                if (!View.SupportsRecentFolders)
                {
                    SettingsManager.Settings.RecentFolders =
                        Enumerable.Repeat(folder, 1).Concat(SettingsManager.Settings.RecentFolders ?? Enumerable.Empty<string>()).ToArray();

                    View.AddRecentFolder(folder);
                }
            }
        }
    }
}
