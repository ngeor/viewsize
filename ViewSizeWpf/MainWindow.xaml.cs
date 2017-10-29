// <copyright file="MainWindow.xaml.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using CRLFLabs.ViewSize.IO;
using CRLFLabs.ViewSize.Mvp;
using CRLFLabs.ViewSizeWpf;

namespace ViewSizeWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Presenter(typeof(MainPresenter))]
    [Presenter(typeof(MenuPresenter))]
    public partial class MainWindow : Window, IMainView, IMenuView
    {
        public MainWindow()
        {
            this.InitializeComponent();
            PresenterFactory.Create(this);
            this.Load?.Invoke(this, EventArgs.Empty);

            this.treeView.DataContext = this.Model;
            new FolderChooserView(this);
            new ApplicationView(this);
        }

        #region IMainView implementation
        public void RunOnGuiThread(Action action) => this.Dispatcher.Invoke(action);

        public void ShowError(Exception ex) => this.ShowError(ex.Message + ex.StackTrace);

        public void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public event EventHandler Load;

        public event EventHandler OnBeginScanClick;

        public event EventHandler OnCancelScanClick;

        public event EventHandler<FileSystemEventArgs> OnTreeViewSelectionChanged;

        public event EventHandler UpOneLevelClick;

        public event EventHandler<CanExecuteEventArgs> UpOneLevelCanExecute;

        public void SetTreeViewContents()
        {
            // not needed for Windows, treeView.DataContext = Model is done once in the constructor
        }

        public IMainModel Model { get; set; }

        public void EnableUI(bool enable)
        {
            this.txtFolder.IsEnabled = enable;
            this.btnScan.IsEnabled = enable;
            this.btnSelectFolder.IsEnabled = enable;
            this.btnCancel.IsEnabled = !enable;
            this.progressBar.IsIndeterminate = !enable;
            this.Cursor = enable ? Cursors.Arrow : Cursors.Wait;
        }

        public void SetScanningItem(string path) => this.lblStatus.Content = path;

        public void SetDurationLabel(string durationLabel) => this.lblDuration.Content = durationLabel;

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

        #region WPF Event Handlers

        private void btnScan_Click(object sender, RoutedEventArgs e)
        {
            this.OnBeginScanClick?.Invoke(this, e);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.OnCancelScanClick?.Invoke(this, e);
        }

        private void treeMap_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var fileSystemEntries = this.Model.Children;
            if (fileSystemEntries != null)
            {
                var point = e.GetPosition(this.treeMap);
                var folder = fileSystemEntries.Find(point.ToPointD());
                this.Model.Selected = folder;
            }
        }

        private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var fileSystemEntry = this.treeView.SelectedItem as FileSystemEntry;
            if (fileSystemEntry == null)
            {
                return;
            }

            this.OnTreeViewSelectionChanged?.Invoke(this, new FileSystemEventArgs(fileSystemEntry));
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
            this.FileSizeTreeMapClick?.Invoke(this, e);
        }

        private void FileCountTreeMap_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void FileCountTreeMap_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.FileCountTreeMapClick?.Invoke(this, e);
        }

        private void ShowInExplorer_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var fileSystemEntry = this.treeView.SelectedItem as FileSystemEntry;
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
            var fileSystemEntry = this.treeView.SelectedItem as FileSystemEntry;
            e.CanExecute = fileSystemEntry != null;
        }

        private void UpOneLevel_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.UpOneLevelClick?.Invoke(this, e);
        }

        private void UpOneLevel_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var args = new CanExecuteEventArgs();
            this.UpOneLevelCanExecute?.Invoke(this, args);
            e.CanExecute = args.CanExecute;
        }

        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        #region IMenuView
        public event EventHandler FileSizeTreeMapClick;

        public event EventHandler FileCountTreeMapClick;

        public event EventHandler FileOpenClick;

        public event EventHandler<RecentFileEventArgs> OpenRecentFileClick;

        public bool IsFileSizeTreeMapChecked
        {
            get => this.mnuFileSizeTreeMap.IsChecked;
            set => this.mnuFileSizeTreeMap.IsChecked = value;
        }

        public bool IsFileCountTreeMapChecked
        {
            get => this.mnuFileCountTreeMap.IsChecked;
            set => this.mnuFileCountTreeMap.IsChecked = value;
        }

        public void ShowMainWindow()
        {
            // not used in Windows
        }
        #endregion
    }
}
