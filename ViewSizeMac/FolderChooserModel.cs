using AppKit;
using CRLFLabs.ViewSize.Mvp;
using System.ComponentModel;

namespace ViewSizeMac
{
    class FolderChooserModel : IFolderChooserModel
    {
        private readonly NSTextField _txtFolder;

        public FolderChooserModel(NSTextField txtFolder)
        {
            _txtFolder = txtFolder;
            _txtFolder.Changed += _txtFolder_Changed;
        }

        public string Folder
        {
            get => _txtFolder.StringValue;
            set
            {
                var newValue = value ?? "";
                var changed = _txtFolder.StringValue != newValue;
                _txtFolder.StringValue = newValue;
                if (changed)
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Folder"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void _txtFolder_Changed(object sender, System.EventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Folder"));
        }
    }
}
