using CRLFLabs.ViewSize.Drawing;
using CRLFLabs.ViewSize.TreeMap;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace CRLFLabs.ViewSize.Mvp
{
    /// <summary>
    /// Main presenter.
    /// </summary>
    public class MainPresenter : IMainPresenter
    {
        private readonly FolderScanner _folderScanner = new FolderScanner();
        private TreeMapDataSource _treeMapDataSource;

        /// <summary>
        /// Creates an instance of this class.
        /// </summary>
        /// <param name="view">The view.</param>
        public MainPresenter(IMainView view)
        {
            View = view;    
        }

        private IMainView View { get; }

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

            View.EnableUI(false);
            var treeMapSize = View.TreeMapActualSize;
            var treeMapWidth = treeMapSize.Width;
            var treeMapHeight = treeMapSize.Height;

            Stopwatch stopwatch = Stopwatch.StartNew();
            Task.Run(() =>
            {
                try
                {
                    var topLevelFolders = _folderScanner.Scan(path);

                    var bounds = new RectangleD(0, 0, treeMapWidth, treeMapHeight);
                    TreeMapDataSource = Renderer.Render(
                        bounds,
                        topLevelFolders);
                    stopwatch.Stop();
                    View.RunOnGuiThread(() =>
                    {
                        View.SetTreeMapDataSource(TreeMapDataSource);
                        View.SetDurationLabel($"Finished scanning in {_folderScanner.Duration}, total time: {stopwatch.Elapsed}");
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
                        View.EnableUI(true);
                    });
                }
            });
        }

        public void OnCancelScan()
        {
            _folderScanner.Cancel();
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
    }
}
