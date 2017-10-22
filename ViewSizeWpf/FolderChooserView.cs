using CRLFLabs.ViewSize.Mvp;
using System;
using System.ComponentModel;
using System.Windows;

namespace ViewSizeWpf
{
    [Presenter(typeof(FolderChooserPresenter))]
    class FolderChooserView : IFolderChooserView
    {
        public FolderChooserView(MainWindow mainWindow)
        {
            // assign references to main window
            MainWindow = mainWindow;
            MainWindow.btnSelectFolder.Click += BtnSelectFolder_Click;
            MainWindow.txtFolder.TextChanged += TxtFolder_TextChanged;

            // start MVP
            PresenterFactory.Create(this);
            Load?.Invoke(this, EventArgs.Empty);
        }

        public IMainModel Model { get; set; }
        private MainWindow MainWindow { get; }

        public event EventHandler OnSelectFolderClick;
        public event EventHandler Load;

        public string Folder
        {
            get => MainWindow.txtFolder.Text;
            set => MainWindow.txtFolder.Text = value;
        }

        public string SelectFolder()
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                var result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    return dialog.SelectedPath;
                }
            }

            return null;
        }

        private void BtnSelectFolder_Click(object sender, RoutedEventArgs e)
        {
            OnSelectFolderClick?.Invoke(this, EventArgs.Empty);
        }

        private void TxtFolder_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            Model.Folder = MainWindow.txtFolder.Text;
        }
    }
}
