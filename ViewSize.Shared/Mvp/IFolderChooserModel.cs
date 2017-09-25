using System.ComponentModel;

namespace CRLFLabs.ViewSize.Mvp
{
    /// <summary>
    /// Folder chooser model.
    /// </summary>
    public interface IFolderChooserModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the selected folder.
        /// </summary>
        /// <value>The selected folder.</value>
        string Folder { get; set; }
    }
}
