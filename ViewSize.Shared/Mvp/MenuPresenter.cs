// <copyright file="MenuPresenter.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;
using System.Linq;
using CRLFLabs.ViewSize.IO;
using CRLFLabs.ViewSize.IoC;
using CRLFLabs.ViewSize.Settings;

namespace CRLFLabs.ViewSize.Mvp
{
    public class MenuPresenter : PresenterBase<IMenuView, IMainModel>, IMenuPresenter
    {
        public MenuPresenter(
            IMenuView view,
            IMainModel model,
            ISettingsManager settingsManager,
            IResolver resolver)
            : base(view, model)
        {
            SettingsManager = settingsManager;
            Resolver = resolver;

            // let the resolver know he can use this instance for IMenuPresenter
            // will be used by other presenters
            resolver.MapExistingInstance(typeof(IMenuPresenter), this);
        }

        private IResolver Resolver { get; }

        private ISettingsManager SettingsManager { get; }

        public void AddRecentFolder(string folder)
        {
            View.AddRecentFolder(folder, insertFirst: true);
        }

        protected override void OnViewLoad(object sender, EventArgs e)
        {
            base.OnViewLoad(sender, e);
            View.FileSizeTreeMapClick += View_FileSizeTreeMapClick;
            View.FileCountTreeMapClick += View_FileCountTreeMapClick;
            View.FileOpenClick += View_FileOpenClick;
            View.OpenRecentFileClick += View_OpenRecentFileClick;

            foreach (var folder in SettingsManager.Settings?.RecentFolders ?? Enumerable.Empty<string>())
            {
                View.AddRecentFolder(folder, insertFirst: false);
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
            Resolver.Resolve<IFolderChooserPresenter>().OpenSelectFolder();
            View.ShowMainWindow();
        }

        private void View_OpenRecentFileClick(object sender, RecentFileEventArgs e)
        {
            Model.Folder = e.Filename;
            View.ShowMainWindow();
        }
    }
}
