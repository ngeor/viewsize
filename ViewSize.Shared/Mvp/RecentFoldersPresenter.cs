// <copyright file="RecentFoldersPresenter.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;

namespace CRLFLabs.ViewSize.Mvp
{
    public class RecentFoldersPresenter : PresenterBase<IRecentFoldersView, IMainModel>
    {
        public RecentFoldersPresenter(IRecentFoldersView view, IMainModel model) : base(view, model)
        {
        }

        protected override void OnViewLoad(object sender, EventArgs e)
        {
            base.OnViewLoad(sender, e);
            View.SetRecentFolders(Model.RecentFolders);
            Model.PropertyChanged += Model_PropertyChanged;
        }

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == MainModel.RecentFoldersPropertyName)
            {
                View.SetRecentFolders(Model.RecentFolders);
            }
        }
    }
}
