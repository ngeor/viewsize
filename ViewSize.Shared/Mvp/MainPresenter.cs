using CRLFLabs.ViewSize.Drawing;
using CRLFLabs.ViewSize.TreeMap;
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
            FileUtils = fileUtils;
            FolderScanner = folderScanner;
            FolderScanner.Scanning += EventThrottler<FileSystemEventArgs>.Throttle(_folderScanner_Scanning);

            Model.PropertyChanging += Model_PropertyChanging;
            Model.PropertyChanged += Model_PropertyChanged;
        }

        private IFolderScanner FolderScanner { get; }
        private IFileUtils FileUtils { get; }

        protected override void OnViewLoad(object sender, EventArgs e)
        {
            base.OnViewLoad(sender, e);
            View.OnBeginScanClick += View_OnBeginScanClick;
            View.OnCancelScanClick += View_OnCancelScanClick;
            View.OnTreeViewSelectionChanged += View_OnTreeViewSelectionChanged;
            View.UpOneLevelClick += View_UpOneLevelClick;
            View.UpOneLevelCanExecute += View_UpOneLevelCanExecute;
        }

        private void View_UpOneLevelCanExecute(object sender, CanExecuteEventArgs e)
        {
            e.CanExecute = Model.Selected?.Parent != null;
        }

        private void View_UpOneLevelClick(object sender, EventArgs e)
        {
            Model.Selected = Model.Selected.Parent;
        }

        void View_OnBeginScanClick(object sender, EventArgs e)
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

        void View_OnCancelScanClick(object sender, EventArgs e)
        {
            FolderScanner.Cancel();
        }

        void View_OnTreeViewSelectionChanged(object sender, FileSystemEventArgs e)
        {
            Model.Selected = e.FileSystemEntry;
        }

        void Model_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
        }

        void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
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
        
        private void _folderScanner_Scanning(object sender, FileSystemEventArgs e)
        {
            View.RunOnGuiThread(() =>
            {
                View.SetScanningItem(e.FileSystemEntry.Path);
            });
        }
    }
}
