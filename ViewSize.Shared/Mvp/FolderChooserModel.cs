using System.ComponentModel;

namespace CRLFLabs.ViewSize.Mvp
{
    public class FolderChooserModel : IFolderChooserModel
    {
        private string _folder;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Folder
        {
            get
            {
                return _folder;
            }

            set
            {
                if (_folder != value)
                {
                    _folder = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Folder"));
                }
            }
        }
    }
}
