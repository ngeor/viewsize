using CRLFLabs.ViewSize;
using CRLFLabs.ViewSize.Drawing;
using CRLFLabs.ViewSize.Mvp;
using CRLFLabs.ViewSize.TreeMap;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ViewSizeWpf;
using ViewSizeWpf.Controls;

namespace ViewSizeWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMainView<FileSystemEntryModel>, IFolderChooserView, IFolderChooserModel
    {
        private readonly MainPresenter<FileSystemEntryModel> mainPresenter;
        private readonly FolderChooserPresenter folderChooserPresenter;

        public MainWindow()
        {
            InitializeComponent();
            mainPresenter = new MainPresenter<FileSystemEntryModel>(this);
            folderChooserPresenter = new FolderChooserPresenter(this, this);
        }

        #region IFolderChooserModel

        string IFolderChooserModel.Folder
        {
            get { return txtFolder.Text; }
            set { txtFolder.Text = value; }
        }

        #endregion

        #region IFolderChooserView

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

        #endregion

        #region IMainView implementation
        public string SelectedFolder
        {
            get { return txtFolder.Text; }
        }

        public void EnableUI(bool enable)
        {
            txtFolder.IsEnabled = enable;
            btnScan.IsEnabled = enable;
            btnSelectFolder.IsEnabled = enable;
            btnCancel.IsEnabled = !enable;
            Cursor = enable ? Cursors.Arrow : Cursors.Wait;
        }

        public SizeD TreeMapActualSize => treeMap.ActualSize;

        public void RunOnGuiThread(Action action) => Dispatcher.Invoke(action);

        public void ShowError(Exception ex) => ShowError(ex.Message + ex.StackTrace);

        public void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void SetDurationLabel(string durationLabel) => lblStatus.Content = durationLabel;

        public void SetFolders(IList<FileSystemEntryModel> topLevelFolders)
        {
            treeView.DataContext = null;
            treeView.DataContext = topLevelFolders;
        }

        public void SetTreeMapDataSource(TreeMapDataSource<FileSystemEntryModel> treeMapDataSource)
        {
            treeMap.DataSource = treeMapDataSource;
        }

        public void SetSelectedTreeViewItem(FileSystemEntryModel selectedFileSystemEntry)
        {
            IList<FileSystemEntryModel> dataSource = treeView.DataContext as IList<FileSystemEntryModel>;
            if (dataSource == null)
            {
                return;
            }
            
            if (selectedFileSystemEntry == null)
            {
                // TODO deselect
                return;
            }

            selectedFileSystemEntry.IsSelected = true;
            for (var n = selectedFileSystemEntry.Parent; n != null; n = n.Parent)
            {
                n.IsExpanded = true;
            }
        }

        #endregion

        #region WPF Event Handlers
        private void btnSelectFolder_Click(object sender, RoutedEventArgs e)
        {
            folderChooserPresenter.OnSelectFolder();
        }

        private void btnScan_Click(object sender, RoutedEventArgs e)
        {
            mainPresenter.OnBeginScan();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            mainPresenter.OnCancelScan();
        }

        private void treeMap_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var dataSource = treeMap.DataSource;
            if (dataSource != null)
            {
                var point = e.GetPosition(treeMap);
                var folder = dataSource.Find(point.ToPointD().Scale(treeMap.ScaleToActual.Invert()));
                dataSource.Selected = folder;
            }
        }

        private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var fileSystemEntry = treeView.SelectedItem as FileSystemEntryModel;
            if (fileSystemEntry == null)
            {
                return;
            }

            mainPresenter.OnTreeViewSelectionChanged(fileSystemEntry);
        }

        #endregion
    }
}
