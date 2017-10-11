using System;

namespace CRLFLabs.ViewSize.Mvp
{
    /// <summary>
    /// Folder chooser view.
    /// </summary>
    public interface IFolderChooserView : IView
    {
        /// <summary>
        /// Occurs when the user clicks the select folder button.
        /// </summary>
        event EventHandler OnSelectFolderClick;

        /// <summary>
        /// Selects the folder.
        /// </summary>
        /// <returns>The folder.</returns>
        string SelectFolder();

        void TriggerSelectFolderClick();
    }
}
