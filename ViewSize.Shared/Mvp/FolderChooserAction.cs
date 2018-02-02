// <copyright file="FolderChooserAction.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;

namespace CRLFLabs.ViewSize.Mvp
{
    public class FolderChooserAction : IFolderChooserAction
    {
        public FolderChooserAction(IMainModel model, Lazy<IFolderChooserView> view)
        {
            Model = model;
            View = view;
        }

        public IMainModel Model { get; }

        public Lazy<IFolderChooserView> View { get; }

        public void SelectFolder()
        {
            string folder = View.Value.SelectFolder();
            if (folder != null)
            {
                Model.Folder = folder;
            }
        }
    }
}
