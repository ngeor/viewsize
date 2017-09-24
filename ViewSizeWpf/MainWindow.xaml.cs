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
    public partial class MainWindow : Window, IMainView, IFolderChooserView, IFolderChooserModel
    {
        private readonly MainPresenter mainPresenter;
        private readonly FolderChooserPresenter folderChooserPresenter;

        public MainWindow()
        {
            InitializeComponent();
            mainPresenter = new MainPresenter(this);
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

        #region IView implementation
        public void RunOnGuiThread(Action action) => Dispatcher.Invoke(action);

        public void ShowError(Exception ex) => ShowError(ex.Message + ex.StackTrace);

        public void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

        public void SetScanningItem(string path) => lblStatus.Content = path;
        public void SetDurationLabel(string durationLabel) => lblDuration.Content = durationLabel;

        public void SetTreeMapDataSource(TreeMapDataSource treeMapDataSource)
        {
            treeMap.DataSource = treeMapDataSource;
            treeView.DataContext = treeMapDataSource;
        }

        public void SetSelectedTreeViewItem(FileSystemEntry selectedFileSystemEntry)
        {
            if (selectedFileSystemEntry == null)
            {
                // TODO deselect
                return;
            }

            selectedFileSystemEntry.IsSelected = true;
            foreach (var ancestor in selectedFileSystemEntry.AncestorsNearestFirst())
            {
                ancestor.IsExpanded = true;
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
            var fileSystemEntry = treeView.SelectedItem as FileSystemEntry;
            if (fileSystemEntry == null)
            {
                return;
            }

            mainPresenter.OnTreeViewSelectionChanged(fileSystemEntry);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            folderChooserPresenter.SaveSettings();
            mainPresenter.SaveSettings();
        }

        #endregion
    }
}
