// <copyright file="MenuPresenter.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;
using CRLFLabs.ViewSize.IO;

namespace CRLFLabs.ViewSize.Mvp
{
    public class MenuPresenter : PresenterBase<IMenuView, IMainModel>, IMenuPresenter
    {
        public MenuPresenter(
            IMenuView view,
            IMainModel model,
            IFolderChooserAction folderChooserAction)
            : base(view, model)
        {
            FolderChooserAction = folderChooserAction;
        }

        private IFolderChooserAction FolderChooserAction { get; }

        protected override void OnViewLoad(object sender, EventArgs e)
        {
            base.OnViewLoad(sender, e);
            View.FileSizeTreeMapClick += View_FileSizeTreeMapClick;
            View.FileCountTreeMapClick += View_FileCountTreeMapClick;
            View.FileOpenClick += View_FileOpenClick;
            View.OpenRecentFileClick += View_OpenRecentFileClick;
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
            FolderChooserAction.SelectFolder();
            View.ShowMainWindow();
        }

        private void View_OpenRecentFileClick(object sender, RecentFileEventArgs e)
        {
            Model.Folder = e.Filename;
            View.ShowMainWindow();
        }
    }
}
