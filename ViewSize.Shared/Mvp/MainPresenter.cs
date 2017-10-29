// <copyright file="MainPresenter.cs" company="CRLFLabs">
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
        /// Creates an instance of this class.
        /// </summary>
        public MainPresenter(IMainView view, IMainModel model, IFolderScanner folderScanner, IFileUtils fileUtils)
            : base(view, model)
        {
            this.FileUtils = fileUtils;
            this.FolderScanner = folderScanner;
            this.FolderScanner.Scanning += EventThrottler<FileSystemEventArgs>.Throttle(this._folderScanner_Scanning);

            this.Model.PropertyChanging += this.Model_PropertyChanging;
            this.Model.PropertyChanged += this.Model_PropertyChanged;
        }

        private IFolderScanner FolderScanner { get; }

        private IFileUtils FileUtils { get; }

        protected override void OnViewLoad(object sender, EventArgs e)
        {
            base.OnViewLoad(sender, e);
            this.View.OnBeginScanClick += this.View_OnBeginScanClick;
            this.View.OnCancelScanClick += this.View_OnCancelScanClick;
            this.View.OnTreeViewSelectionChanged += this.View_OnTreeViewSelectionChanged;
            this.View.UpOneLevelClick += this.View_UpOneLevelClick;
            this.View.UpOneLevelCanExecute += this.View_UpOneLevelCanExecute;
        }

        private void View_UpOneLevelCanExecute(object sender, CanExecuteEventArgs e)
        {
            e.CanExecute = this.Model.Selected?.Parent != null;
        }

        private void View_UpOneLevelClick(object sender, EventArgs e)
        {
            var parent = this.Model.Selected?.Parent;
            if (parent != null)
            {
                // need to check if it is null because on Mac the CanExecute event is not being used
                this.Model.Selected = parent;
            }
        }

        private void View_OnBeginScanClick(object sender, EventArgs e)
        {
            string path = this.Model.Folder;
            if (string.IsNullOrWhiteSpace(path))
            {
                this.View.ShowError("No folder selected!");
                return;
            }

            if (!this.FileUtils.IsDirectory(path))
            {
                this.View.ShowError($"Folder '{path}' does not exist!");
                return;
            }

            this.Model.IsScanning = true;
            this.View.EnableUI(false);

            Stopwatch stopwatch = Stopwatch.StartNew();

            // progress task
            Task.Run(async () =>
            {
                while (this.Model.IsScanning)
                {
                    this.View.RunOnGuiThread(() =>
                    {
                        this.View.SetDurationLabel(stopwatch.Elapsed.ToString("mm\\:ss"));
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
                    this.Model.TopLevelFolders = this.FolderScanner.Scan(path);
                    stopwatch.Stop();
                    this.View.RunOnGuiThread(() =>
                    {
                        this.Model.Selected = null;
                        this.Model.Children = this.Model.TopLevelFolders;
                    });
                }
                catch (Exception ex)
                {
                    this.View.RunOnGuiThread(() => this.View.ShowError(ex));
                }
                finally
                {
                    this.View.RunOnGuiThread(() =>
                    {
                        this.Model.IsScanning = false;
                        this.View.EnableUI(true);
                    });
                }
            });
        }

        private void View_OnCancelScanClick(object sender, EventArgs e)
        {
            this.FolderScanner.Cancel();
        }

        private void View_OnTreeViewSelectionChanged(object sender, FileSystemEventArgs e)
        {
            this.Model.Selected = e.FileSystemEntry;
        }

        private void Model_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
        }

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case MainModel.SelectedPropertyName:
                    this.View.SetSelectedTreeViewItem(this.Model.Selected);
                    break;
                case MainModel.ChildrenPropertyName:
                    this.View.SetTreeViewContents();
                    break;
            }
        }

        private void _folderScanner_Scanning(object sender, FileSystemEventArgs e)
        {
            this.View.RunOnGuiThread(() =>
            {
                this.View.SetScanningItem(e.FileSystemEntry.Path);
            });
        }
    }
}
