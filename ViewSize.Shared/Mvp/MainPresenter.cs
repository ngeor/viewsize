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
        private bool _isScanning;
        private RectangleD _lastBounds;
        private SortKey _lastSortKey;

        /// <summary>
        /// Creates an instance of this class.
        /// </summary>
        public MainPresenter(IMainView view, IMainModel model, IFolderScanner folderScanner, IFileUtils fileUtils)
            : base(view, model)
        {
            FileUtils = fileUtils;
            FolderScanner = folderScanner;
            FolderScanner.Scanning += EventThrottler<FileSystemEventArgs>.Throttle(_folderScanner_Scanning);
        }

        private IFolderScanner FolderScanner { get; }
        private IFileUtils FileUtils { get; }

        protected override void AttachToView()
        {
            View.OnBeginScanClick += View_OnBeginScanClick;
            View.OnCancelScanClick += View_OnCancelScanClick;
            View.OnTreeViewSelectionChanged += View_OnTreeViewSelectionChanged;
            View.TreeMapView.RedrawNeeded += TreeMapView_RedrawNeeded;
        }

        protected override void AttachToModel()
        {
            Model.PropertyChanging += Model_PropertyChanging;
            Model.PropertyChanged += Model_PropertyChanged;
        }

        void View_OnBeginScanClick(object sender, EventArgs e)
        {
            string path = View.SelectedFolder;
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

            _isScanning = true;
            View.EnableUI(false);
            var bounds = View.TreeMapView.BoundsD;
            var treeMapWidth = bounds.Width;
            var treeMapHeight = bounds.Height;

            Stopwatch stopwatch = Stopwatch.StartNew();

            // progress task
            Task.Run(async () =>
            {
                while (_isScanning)
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
                    var topLevelFolders = FolderScanner.Scan(path);
                    var renderer = new Renderer(bounds, topLevelFolders, Model.SortKey);
                    renderer.Render();
                    stopwatch.Stop();
                    View.RunOnGuiThread(() =>
                    {
                        // capture these two for recalculate
                        _lastBounds = bounds;
                        _lastSortKey = Model.SortKey;

                        Model.Children = topLevelFolders;
                    });
                }
                catch (Exception ex)
                {
                    View.RunOnGuiThread(() => View.ShowError(ex));
                }
                finally
                {
                    _isScanning = false;
                    View.RunOnGuiThread(() =>
                    {
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

        void TreeMapView_RedrawNeeded(object sender, EventArgs e)
        {
            ReCalculateTreeMap(false);
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
                case MainModel.SortKeyPropertyName:
                    ReCalculateTreeMap(true);
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

        private void ReCalculateTreeMap(bool forceRedraw)
        {
            if (Model.Children == null)
            {
                // no data yet
                return;
            }

            var bounds = View.TreeMapView.BoundsD;
            if (bounds.Size == _lastBounds.Size && Model.SortKey == _lastSortKey)
            {
                return;
            }

            _lastBounds = bounds;
            _lastSortKey = Model.SortKey;

            var renderer = new Renderer(bounds, Model.Children, Model.SortKey);
            renderer.Render();

            if (forceRedraw)
            {
                View.TreeMapView.Redraw();
            }
        }
    }
}
