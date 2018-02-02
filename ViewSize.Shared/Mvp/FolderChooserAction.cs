// <copyright file="FolderChooserAction.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

namespace CRLFLabs.ViewSize.Mvp
{
    public class FolderChooserAction : IFolderChooserAction
    {
        public FolderChooserAction(IMainModel model, IFolderChooserView view)
        {
            Model = model;
            View = view;
        }

        public IMainModel Model { get; }

        public IFolderChooserView View { get; }

        public void SelectFolder()
        {
            string folder = View.SelectFolder();
            if (folder != null)
            {
                Model.Folder = folder;
            }
        }
    }
}
