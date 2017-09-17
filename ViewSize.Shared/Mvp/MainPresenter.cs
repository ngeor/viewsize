using CRLFLabs.ViewSize.Drawing;
using CRLFLabs.ViewSize.TreeMap;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace CRLFLabs.ViewSize.Mvp
{

    /// <summary>
    /// Main presenter.
    /// </summary>
    public class MainPresenter : IMainPresenter
    {
        private readonly FolderScanner _folderScanner = new FolderScanner();
        private TreeMapDataSource _treeMapDataSource;

        public MainPresenter(IMainView view)
        {
            View = view;    
        }

        private IMainView View { get; }

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
                    _treeMapDataSource = Renderer.Render(bounds, _folderScanner.TopLevelFolders);
                    stopwatch.Stop();
                    View.RunOnGuiThread(() =>
                    {
                        View.SetFolders(_folderScanner.TopLevelFolders);
                        View.SetTreeMapDataSource(_treeMapDataSource);
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
    }
}
