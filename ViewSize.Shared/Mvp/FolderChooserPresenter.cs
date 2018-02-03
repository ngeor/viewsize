// <copyright file="FolderChooserPresenter.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;

namespace CRLFLabs.ViewSize.Mvp
{
    /// <summary>
    /// Folder chooser presenter.
    /// </summary>
    public class FolderChooserPresenter : PresenterBase<IFolderChooserView, IMainModel>
    {
        public FolderChooserPresenter(
            IFolderChooserView view,
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
            InitModelFromSettings();
            SubscribeToViewEvents();
            InitViewFromModel();
        }

        private void InitModelFromSettings()
        {
            Model.PropertyChanged += Model_PropertyChanged;
        }

        private void InitViewFromModel()
        {
            View.Folder = Model.Folder;
        }

        private void SubscribeToViewEvents()
        {
            View.SelectFolderClick += (_, e) => FolderChooserAction.SelectFolder();
        }

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == MainModel.FolderPropertyName)
            {
                InitViewFromModel();
            }
        }
    }
}
