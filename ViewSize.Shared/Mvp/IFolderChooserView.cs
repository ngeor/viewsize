using System;

namespace CRLFLabs.ViewSize.Mvp
{
    /// <summary>
    /// Folder chooser view.
    /// </summary>
    public interface IFolderChooserView : IView
    {
        /// <summary>
        /// Selects the folder.
        /// </summary>
        /// <returns>The folder.</returns>
        string SelectFolder();
    }
}
