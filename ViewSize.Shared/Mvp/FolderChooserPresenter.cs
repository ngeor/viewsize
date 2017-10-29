// <copyright file="FolderChooserPresenter.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

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
            this.SettingsManager = settingsManager;
            this.Model.Folder = settingsManager.Settings.SelectedFolder;
            this.Model.PropertyChanged += this.Model_PropertyChanged;
            commandBus.Subscribe("SelectFolder", () => this.View_OnSelectFolderClick(null, EventArgs.Empty));
        }

        private ISettingsManager SettingsManager { get; }

        protected override void OnViewLoad(object sender, EventArgs e)
        {
            base.OnViewLoad(sender, e);
            this.View.OnSelectFolderClick += this.View_OnSelectFolderClick;
            this.View.Folder = this.Model.Folder;
        }

        private void View_OnSelectFolderClick(object sender, EventArgs e)
        {
            string folder = this.View.SelectFolder();
            if (folder != null)
            {
                this.Model.Folder = folder;
            }
        }

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == MainModel.FolderPropertyName)
            {
                this.View.Folder = this.Model.Folder;
                this.SettingsManager.Settings.SelectedFolder = this.Model.Folder;
            }
        }
    }
}
