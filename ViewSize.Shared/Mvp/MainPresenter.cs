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
    public class MainPresenter<T> : IMainPresenter
        where T : IFileSystemEntry, new()
    {
        private readonly FolderScanner<T> _folderScanner = new FolderScanner<T>();
        private TreeMapDataSource _treeMapDataSource;

        /// <summary>
        /// Creates an instance of this class.
        /// </summary>
        /// <param name="view">The view.</param>
        public MainPresenter(IMainView<T> view)
        {
            View = view;    
        }

        private IMainView<T> View { get; }

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
            View.EnableUI(false);
            string path = View.SelectedFolder;
            var treeMapSize = View.TreeMapActualSize;
            var treeMapWidth = treeMapSize.Width;
            var treeMapHeight = treeMapSize.Height;

            Stopwatch stopwatch = Stopwatch.StartNew();
            Task.Run(() =>
            {
                try
                {
                    _folderScanner.Scan(path);

                    var bounds = new RectangleD(0, 0, treeMapWidth, treeMapHeight);
                    TreeMapDataSource = Renderer.Render(
                        bounds,
                        _folderScanner.TopLevelFolders.Cast<IFileSystemEntry>().ToList()); // TODO: can we avoid the cast?
                    stopwatch.Stop();
                    View.RunOnGuiThread(() =>
                    {
                        View.SetFolders(_folderScanner.TopLevelFolders);
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

        public void OnTreeViewSelectionChanged(string path)
        {
            var folderWithDrawSize = _treeMapDataSource?.Find(path);
            _treeMapDataSource.Selected = folderWithDrawSize;
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
            View.SetSelectedTreeViewItem(TreeMapDataSource.Selected?.FileSystemEntry?.Path);
        }
    }
}
