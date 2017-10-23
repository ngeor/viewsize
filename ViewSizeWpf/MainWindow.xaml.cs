﻿using CRLFLabs.ViewSize.IO;
using CRLFLabs.ViewSize.Mvp;
using CRLFLabs.ViewSizeWpf;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

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
            InitializeComponent();
            PresenterFactory.Create(this);
            Load?.Invoke(this, EventArgs.Empty);

            treeView.DataContext = Model;
            new FolderChooserView(this);
            new ApplicationView(this);
        }

        #region IMainView implementation
        public void RunOnGuiThread(Action action) => Dispatcher.Invoke(action);

        public void ShowError(Exception ex) => ShowError(ex.Message + ex.StackTrace);

        public void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        
        public event EventHandler Load;
        public event EventHandler OnBeginScanClick;
        public event EventHandler OnCancelScanClick;
        public event EventHandler<FileSystemEventArgs> OnTreeViewSelectionChanged;

        public void SetTreeViewContents()
        {
            // not needed for Windows, treeView.DataContext = Model is done once in the constructor
        }

        public IMainModel Model { get; set; }

        public void EnableUI(bool enable)
        {
            txtFolder.IsEnabled = enable;
            btnScan.IsEnabled = enable;
            btnSelectFolder.IsEnabled = enable;
            btnCancel.IsEnabled = !enable;
            progressBar.IsIndeterminate = !enable;
            Cursor = enable ? Cursors.Arrow : Cursors.Wait;
        }

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

        #region WPF Event Handlers

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
            var fileSystemEntries = Model.Children;
            if (fileSystemEntries != null)
            {
                var point = e.GetPosition(treeMap);
                var folder = fileSystemEntries.Find(point.ToPointD());
                Model.Selected = folder;
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
            FileSizeTreeMapClick?.Invoke(this, e);
        }

        private void FileCountTreeMap_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void FileCountTreeMap_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FileCountTreeMapClick?.Invoke(this, e);
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
            Model.Selected = Model.Selected.Parent as FileSystemEntry;
        }

        private void UpOneLevel_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var selected = Model?.Selected;
            e.CanExecute = selected != null && selected.Parent is FileSystemEntry;
        }

        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        #region IMenuView
        public event EventHandler FileSizeTreeMapClick;
        public event EventHandler FileCountTreeMapClick;

        public bool IsFileSizeTreeMapChecked
        {
            get => mnuFileSizeTreeMap.IsChecked;
            set => mnuFileSizeTreeMap.IsChecked = value;
        }

        public bool IsFileCountTreeMapChecked
        {
            get => mnuFileCountTreeMap.IsChecked;
            set => mnuFileCountTreeMap.IsChecked = value;
        }
        #endregion
    }
}
