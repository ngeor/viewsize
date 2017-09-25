﻿using CRLFLabs.ViewSize.IO;

namespace CRLFLabs.ViewSize.Mvp
{
    public interface IMainPresenter
    {
        /// <summary>
        /// Occurs when the user wants to start the folder scan.
        /// </summary>
        void OnBeginScan();

        /// <summary>
        /// Occurs when the user wants to abort the folder scan.
        /// </summary>
        void OnCancelScan();

        /// <summary>
        /// Occurs when the user has selected a file system entry on the tree view.
        /// </summary>
        /// <param name="selection">The selected file system entry.</param>
        void OnTreeViewSelectionChanged(FileSystemEntry selection);
    }
}
