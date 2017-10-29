// <copyright file="MenuPresenter.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;
using CRLFLabs.ViewSize.IO;

namespace CRLFLabs.ViewSize.Mvp
{
    public class MenuPresenter : PresenterBase<IMenuView, IMainModel>
    {
        public MenuPresenter(IMenuView view, IMainModel model, ICommandBus commandBus)
            : base(view, model)
        {
            this.CommandBus = commandBus;
        }

        private ICommandBus CommandBus { get; }

        protected override void OnViewLoad(object sender, EventArgs e)
        {
            base.OnViewLoad(sender, e);
            this.View.FileSizeTreeMapClick += this.View_FileSizeTreeMapClick;
            this.View.FileCountTreeMapClick += this.View_FileCountTreeMapClick;
            this.View.FileOpenClick += this.View_FileOpenClick;
            this.View.OpenRecentFileClick += this.View_OpenRecentFileClick;
        }

        private void View_FileSizeTreeMapClick(object sender, EventArgs e)
        {
            this.View.IsFileSizeTreeMapChecked = true;
            this.View.IsFileCountTreeMapChecked = false;
            this.Model.SortKey = SortKey.Size;
        }

        private void View_FileCountTreeMapClick(object sender, EventArgs e)
        {
            this.View.IsFileSizeTreeMapChecked = false;
            this.View.IsFileCountTreeMapChecked = true;
            this.Model.SortKey = SortKey.Count;
        }

        private void View_FileOpenClick(object sender, EventArgs e)
        {
            this.CommandBus.Publish("SelectFolder");
            this.View.ShowMainWindow();
        }

        private void View_OpenRecentFileClick(object sender, RecentFileEventArgs e)
        {
            this.Model.Folder = e.Filename;
            this.View.ShowMainWindow();
        }
    }
}
