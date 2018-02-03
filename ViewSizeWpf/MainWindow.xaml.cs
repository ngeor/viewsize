// <copyright file="MainWindow.xaml.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
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
    [Presenter(typeof(RecentFoldersPresenter))]
    public partial class MainWindow : Window, IMainView, IMenuView, IRecentFoldersView
    {
#pragma warning disable SA1401 // Fields must be private
        public static RoutedCommand ShowInExplorerCommand = new RoutedCommand();
        public static RoutedCommand UpOneLevelCommand = new RoutedCommand();
        public static RoutedCommand FileSizeTreeMapCommand = new RoutedCommand();
        public static RoutedCommand FileCountTreeMapCommand = new RoutedCommand();
        public static RoutedCommand ClearRecentFoldersCommand = new RoutedCommand();
#pragma warning restore SA1401 // Fields must be private

        public MainWindow()
        {
            InitializeComponent();
            PresenterFactory.Create(this);
            Load?.Invoke(this, EventArgs.Empty);

            treeView.DataContext = Model;
            new FolderChooserView(this);
            new ApplicationView(this);
        }

        public event EventHandler FileSizeTreeMapClick;

        public event EventHandler FileCountTreeMapClick;

        public event EventHandler FileOpenClick;

        public event EventHandler<RecentFileEventArgs> OpenRecentFileClick;

        public event EventHandler Load;

        public event EventHandler BeginScanClick;

        public event EventHandler CancelScanClick;

        public event EventHandler<FileSystemEventArgs> TreeViewSelectionChanged;

        public event EventHandler UpOneLevelClick;

        public event EventHandler<CanExecuteEventArgs> UpOneLevelCanExecute;

        public IMainModel Model { get; set; }

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

        public void RunOnGuiThread(Action action) => Dispatcher.Invoke(action);

        public void ShowError(Exception ex) => ShowError(ex.Message + ex.StackTrace);

        public void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void SetTreeViewContents()
        {
            // not needed for Windows, treeView.DataContext = Model is done once in the constructor
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

        public void ShowMainWindow()
        {
            // not used in Windows
        }

        public void SetRecentFolders(IEnumerable<string> folders)
        {
            while (mnuRecentFolders.Items.Count > 2)
            {
                mnuRecentFolders.Items.RemoveAt(0);
            }

            foreach (var folder in folders)
            {
                var menuItem = new MenuItem
                {
                    Header = folder
                };

                menuItem.Click += (s, a) => OpenRecentFileClick?.Invoke(this, new RecentFileEventArgs(folder));

                // -2 because of the separator and the 'clear recent folders' entry
                var position = mnuRecentFolders.Items.Count - 2;
                mnuRecentFolders.Items.Insert(position, menuItem);
            }
        }

        private void btnScan_Click(object sender, RoutedEventArgs e)
        {
            BeginScanClick?.Invoke(this, e);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            CancelScanClick?.Invoke(this, e);
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

            TreeViewSelectionChanged?.Invoke(this, new FileSystemEventArgs(fileSystemEntry));
        }

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

        private void ShowInExplorer_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var fileSystemEntry = treeView.SelectedItem as FileSystemEntry;
            e.CanExecute = fileSystemEntry != null;
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

        private void UpOneLevel_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            UpOneLevelClick?.Invoke(this, e);
        }

        private void UpOneLevel_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var args = new CanExecuteEventArgs();
            UpOneLevelCanExecute?.Invoke(this, args);
            e.CanExecute = args.CanExecute;
        }

        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FileOpenClick?.Invoke(this, EventArgs.Empty);
        }

        private void ClearRecentFolders_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            // since this is Windows only, it is implemented directly here
            e.CanExecute = Model.RecentFolders.Count > 0;
        }

        private void ClearRecentFolders_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // since this is Windows only, it is implemented directly here
            Model.RecentFolders = new string[0];
        }
    }
}
