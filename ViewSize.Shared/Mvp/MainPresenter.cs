﻿// <copyright file="MainPresenter.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using CRLFLabs.ViewSize.IO;

namespace CRLFLabs.ViewSize.Mvp
{
    /// <summary>
    /// Main presenter.
    /// </summary>
    public class MainPresenter : PresenterBase<IMainView, IMainModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainPresenter"/> class.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="model">The model.</param>
        /// <param name="folderScanner">The folder scanner.</param>
        /// <param name="fileUtils">The file utilities.</param>
        public MainPresenter(IMainView view, IMainModel model, IFolderScanner folderScanner, IFileUtils fileUtils)
            : base(view, model)
        {
            FileUtils = fileUtils;
            FolderScanner = folderScanner;
            FolderScanner.Scanning += EventThrottler<FileSystemEventArgs>.Throttle(Scanning);

            Model.PropertyChanging += Model_PropertyChanging;
            Model.PropertyChanged += Model_PropertyChanged;
        }

        private IFolderScanner FolderScanner { get; }

        private IFileUtils FileUtils { get; }

        protected override void OnViewLoad(object sender, EventArgs e)
        {
            base.OnViewLoad(sender, e);
            View.BeginScanClick += BeginScanClick;
            View.CancelScanClick += CancelScanClick;
            View.TreeViewSelectionChanged += TreeViewSelectionChanged;
            View.UpOneLevelClick += UpOneLevelClick;
            View.UpOneLevelCanExecute += UpOneLevelCanExecute;
        }

        private void UpOneLevelCanExecute(object sender, CanExecuteEventArgs e)
        {
            e.CanExecute = Model.Selected?.Parent != null;
        }

        private void UpOneLevelClick(object sender, EventArgs e)
        {
            var parent = Model.Selected?.Parent;
            if (parent != null)
            {
                // need to check if it is null because on Mac the CanExecute event is not being used
                Model.Selected = parent;
            }
        }

        private void BeginScanClick(object sender, EventArgs e)
        {
            string path = Model.Folder;
            if (string.IsNullOrWhiteSpace(path))
            {
                View.ShowError("No folder selected!");
                return;
            }

            if (!FileUtils.IsDirectory(path))
            {
                View.ShowError($"Folder '{path}' does not exist!");
                return;
            }

            Model.IsScanning = true;
            View.EnableUI(false);

            Stopwatch stopwatch = Stopwatch.StartNew();

            // progress task
            Task.Run(async () =>
            {
                while (Model.IsScanning)
                {
                    View.RunOnGuiThread(() =>
                    {
                        View.SetDurationLabel(stopwatch.Elapsed.ToString("mm\\:ss"));
                    });

                    await Task.Delay(1000);
                }
            });

            // main task
            Task.Run(() =>
            {
                try
                {
                    // this is picked up by TreeMapPresenter
                    Model.TopLevelFolders = FolderScanner.Scan(path);
                    stopwatch.Stop();
                    View.RunOnGuiThread(() =>
                    {
                        Model.Selected = null;
                        Model.Children = Model.TopLevelFolders;
                    });
                }
                catch (Exception ex)
                {
                    View.RunOnGuiThread(() => View.ShowError(ex));
                }
                finally
                {
                    View.RunOnGuiThread(() =>
                    {
                        Model.IsScanning = false;
                        View.EnableUI(true);
                    });
                }
            });
        }

        private void CancelScanClick(object sender, EventArgs e)
        {
            FolderScanner.Cancel();
        }

        private void TreeViewSelectionChanged(object sender, FileSystemEventArgs e)
        {
            Model.Selected = e.FileSystemEntry;
        }

        private void Model_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
        }

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case MainModel.SelectedPropertyName:
                    View.SetSelectedTreeViewItem(Model.Selected);
                    break;
                case MainModel.ChildrenPropertyName:
                    View.SetTreeViewContents();
                    break;
            }
        }

        /// <summary>
        /// Called when folder scanner scans a new item and updates the UI accordingly.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void Scanning(object sender, FileSystemEventArgs e)
        {
            View.RunOnGuiThread(() =>
            {
                View.SetScanningItem(e.FileSystemEntry.Path);
            });
        }
    }
}
