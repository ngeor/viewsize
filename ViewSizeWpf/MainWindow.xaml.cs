using CRLFLabs.ViewSize.Drawing;
using CRLFLabs.ViewSize.IO;
using CRLFLabs.ViewSize.Mvp;
using CRLFLabs.ViewSize.Settings;
using CRLFLabs.ViewSize.TreeMap;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace ViewSizeWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMainView, IFolderChooserView, IFolderChooserModel, IApplicationView
    {
        private readonly MainModel _mainModel;

        public MainWindow()
        {
            InitializeComponent();

            var fileUtils = new FileUtils();
            var folderScanner = new FolderScanner(fileUtils);
            _mainModel = new MainModel();
            _mainModel.PropertyChanged += MainModel_PropertyChanged;
            var mainPresenter = new MainPresenter(this, _mainModel, folderScanner, fileUtils);

            var settingsManager = SettingsManager.Instance;
            var folderChooserPresenter = new FolderChooserPresenter(this, this, settingsManager);

            var applicationPresenter = new ApplicationPresenter(this, settingsManager);

            treeMap.Model = _mainModel;
        }

        #region IFolderChooserModel

        public event PropertyChangedEventHandler PropertyChanged;

        string IFolderChooserModel.Folder
        {
            get { return txtFolder.Text; }
            set { txtFolder.Text = value; }
        }

        #endregion

        #region IFolderChooserView

        public event EventHandler OnSelectFolderClick;

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

        private void MainModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == MainModel.ChildrenPropertyName)
            {
                treeView.DataContext = ((MainModel)sender);
            }
        }

        #region IMainView implementation

        public event EventHandler OnBeginScanClick;
        public event EventHandler OnCancelScanClick;
        public event EventHandler<FileSystemEventArgs> OnTreeViewSelectionChanged;

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
            progressBar.IsIndeterminate = !enable;
            Cursor = enable ? Cursors.Arrow : Cursors.Wait;
        }

        public ITreeMapView TreeMapView => treeMap;

        public void SetScanningItem(string path) => lblStatus.Content = path;
        public void SetDurationLabel(string durationLabel) => lblDuration.Content = durationLabel;

        public void SetSelectedTreeViewItem(FileSystemEntry selectedFileSystemEntry)
        {
            if (selectedFileSystemEntry == null)
            {
                // TODO deselect
                return;
            }

            selectedFileSystemEntry.IsSelected = true;
            foreach (var ancestor in selectedFileSystemEntry.Ancestors())
            {
                ancestor.IsExpanded = true;
            }
        }

        #endregion

        #region IApplicationView

        private event EventHandler ApplicationView_Closing;
        event EventHandler IApplicationView.Closing
        {
            add => ApplicationView_Closing += value;
            remove => ApplicationView_Closing -= value;
        }
        #endregion

        #region WPF Event Handlers
        private void txtFolder_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Folder"));
        }

        private void btnSelectFolder_Click(object sender, RoutedEventArgs e)
        {
            OnSelectFolderClick?.Invoke(this, e);
        }

        private void btnScan_Click(object sender, RoutedEventArgs e)
        {
            OnBeginScanClick?.Invoke(this, e);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            OnCancelScanClick?.Invoke(this, e);
        }

        private void treeMap_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var fileSystemEntries = _mainModel.Children;
            if (fileSystemEntries != null)
            {
                var point = e.GetPosition(treeMap);
                var folder = fileSystemEntries.Find(point.ToPointD());
                _mainModel.Selected = folder;
            }
        }

        private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var fileSystemEntry = treeView.SelectedItem as FileSystemEntry;
            if (fileSystemEntry == null)
            {
                return;
            }

            OnTreeViewSelectionChanged?.Invoke(this, new FileSystemEventArgs(fileSystemEntry));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ApplicationView_Closing?.Invoke(this, e);
        }

        #endregion

        public static RoutedCommand ShowInExplorerCommand = new RoutedCommand();
        public static RoutedCommand UpOneLevelCommand = new RoutedCommand();
        public static RoutedCommand FileSizeTreeMapCommand = new RoutedCommand();
        public static RoutedCommand FileCountTreeMapCommand = new RoutedCommand();

        private void FileSizeTreeMap_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void FileSizeTreeMap_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            mnuFileCountTreeMap.IsChecked = false;
            mnuFileSizeTreeMap.IsChecked = true;
            _mainModel.SortKey = SortKey.Size;
        }

        private void FileCountTreeMap_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void FileCountTreeMap_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            mnuFileCountTreeMap.IsChecked = true;
            mnuFileSizeTreeMap.IsChecked = false;
            _mainModel.SortKey = SortKey.Count;
        }

        private void ShowInExplorer_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var fileSystemEntry = treeView.SelectedItem as FileSystemEntry;
            string path;
            if (!fileSystemEntry.IsDirectory)
            {
                path = System.IO.Path.GetDirectoryName(fileSystemEntry.Path);
            }
            else
            {
                path = fileSystemEntry.Path;
            }

            Process.Start(path);
        }

        private void ShowInExplorer_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var fileSystemEntry = treeView.SelectedItem as FileSystemEntry;
            e.CanExecute = fileSystemEntry != null;
        }

        private void UpOneLevel_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _mainModel.Selected = _mainModel.Selected.Parent as FileSystemEntry;
        }

        private void UpOneLevel_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var selected = _mainModel?.Selected;
            e.CanExecute = selected != null && selected.Parent is FileSystemEntry;
        }

        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }
    }
}
