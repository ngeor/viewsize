// <copyright file="MenuPresenter.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;
using System.Linq;
using CRLFLabs.ViewSize.IO;
using CRLFLabs.ViewSize.Settings;

namespace CRLFLabs.ViewSize.Mvp
{
    public class MenuPresenter : PresenterBase<IMenuView, IMainModel>
    {
        public MenuPresenter(IMenuView view, IMainModel model, ICommandBus commandBus, ISettingsManager settingsManager)
            : base(view, model)
        {
            CommandBus = commandBus;
            SettingsManager = settingsManager;
        }

        private ICommandBus CommandBus { get; }

        private ISettingsManager SettingsManager { get; }

        protected override void OnViewLoad(object sender, EventArgs e)
        {
            base.OnViewLoad(sender, e);
            View.FileSizeTreeMapClick += View_FileSizeTreeMapClick;
            View.FileCountTreeMapClick += View_FileCountTreeMapClick;
            View.FileOpenClick += View_FileOpenClick;
            View.OpenRecentFileClick += View_OpenRecentFileClick;

            foreach (var folder in SettingsManager.Settings?.RecentFolders ?? Enumerable.Empty<string>())
            {
                View.AddRecentFolder(folder);
            }
        }

        private void View_FileSizeTreeMapClick(object sender, EventArgs e)
        {
            View.IsFileSizeTreeMapChecked = true;
            View.IsFileCountTreeMapChecked = false;
            Model.SortKey = SortKey.Size;
        }

        private void View_FileCountTreeMapClick(object sender, EventArgs e)
        {
            View.IsFileSizeTreeMapChecked = false;
            View.IsFileCountTreeMapChecked = true;
            Model.SortKey = SortKey.Count;
        }

        private void View_FileOpenClick(object sender, EventArgs e)
        {
            CommandBus.Publish("SelectFolder");
            View.ShowMainWindow();
        }

        private void View_OpenRecentFileClick(object sender, RecentFileEventArgs e)
        {
            Model.Folder = e.Filename;
            View.ShowMainWindow();
        }
    }
}
