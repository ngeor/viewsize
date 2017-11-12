// <copyright file="IFolderChooserView.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;

namespace CRLFLabs.ViewSize.Mvp
{
    /// <summary>
    /// Folder chooser view.
    /// </summary>
    public interface IFolderChooserView : IView<IMainModel>
    {
        /// <summary>
        /// Occurs when the user clicks the select folder button.
        /// </summary>
        event EventHandler OnSelectFolderClick;

        /// <summary>
        /// Gets a value indicating whether this view handles recent folder storage automatically.
        /// </summary>
        bool SupportsRecentFolders { get; }

        /// <summary>
        /// Gets or sets the selected folder.
        /// </summary>
        /// <value>The folder.</value>
        string Folder { get; set; }

        /// <summary>
        /// Selects the folder.
        /// </summary>
        /// <returns>The folder.</returns>
        string SelectFolder();

        /// <summary>
        /// Adds a recent folder to the recent folder menu.
        /// </summary>
        /// <param name="folder">The recent folder.</param>
        void AddRecentFolder(string folder);
    }
}
