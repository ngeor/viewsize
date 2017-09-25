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
    public class MainPresenter : IMainPresenter
    {
        private TreeMapDataSource _treeMapDataSource;
        private bool _isScanning;

        /// <summary>
        /// Creates an instance of this class.
        /// </summary>
        /// <param name="view">The view.</param>
        public MainPresenter(IMainView view, IFolderScanner folderScanner, IFileUtils fileUtils)
        {
            FileUtils = fileUtils;
            View = view;
            FolderScanner = folderScanner;
            FolderScanner.Scanning += EventThrottler<FileSystemEventArgs>.Throttle(_folderScanner_Scanning);
        }

        private IFolderScanner FolderScanner { get; }
        private IMainView View { get; }
        private IFileUtils FileUtils { get; }

        private TreeMapDataSource TreeMapDataSource
        {
            get
            {
                return _treeMapDataSource;
            }
            set
            {
                Detach(_treeMapDataSource);
                _treeMapDataSource = value;
                Attach(_treeMapDataSource);
            }
        }

        public void OnBeginScan()
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
            var treeMapSize = View.TreeMapActualSize;
            var treeMapWidth = treeMapSize.Width;
            var treeMapHeight = treeMapSize.Height;

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

                    var bounds = new RectangleD(0, 0, treeMapWidth, treeMapHeight);
                    TreeMapDataSource = Renderer.Render(
                        bounds,
                        topLevelFolders);
                    stopwatch.Stop();
                    View.RunOnGuiThread(() =>
                    {
                        View.SetTreeMapDataSource(TreeMapDataSource);
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

        public void OnCancelScan()
        {
            FolderScanner.Cancel();
        }

        public void OnTreeViewSelectionChanged(FileSystemEntry selection)
        {
            _treeMapDataSource.Selected = selection;
        }

        private void Attach(TreeMapDataSource treeMapDataSource)
        {
            if (treeMapDataSource != null)
            {
                treeMapDataSource.PropertyChanged += TreeMapDataSource_PropertyChanged;
            }
        }

        private void Detach(TreeMapDataSource treeMapDataSource)
        {
            if (treeMapDataSource != null)
            {
                treeMapDataSource.PropertyChanged -= TreeMapDataSource_PropertyChanged;
            }
        }

        private void TreeMapDataSource_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            View.SetSelectedTreeViewItem(TreeMapDataSource.Selected);
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
