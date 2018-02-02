// <copyright file="FolderChooserPresenter.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Linq;
using CRLFLabs.ViewSize.IoC;
using CRLFLabs.ViewSize.Settings;

namespace CRLFLabs.ViewSize.Mvp
{
    /// <summary>
    /// Folder chooser presenter.
    /// </summary>
    public class FolderChooserPresenter : PresenterBase<IFolderChooserView, IMainModel>, IFolderChooserPresenter
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
            View.OnSelectFolderClick += View_OnSelectFolderClick;
        }

        private void View_OnSelectFolderClick(object sender, EventArgs e)
        {
            FolderChooserAction.SelectFolder();
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
