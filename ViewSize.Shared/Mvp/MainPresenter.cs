using CRLFLabs.ViewSize.Drawing;
using CRLFLabs.ViewSize.TreeMap;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace CRLFLabs.ViewSize.Mvp
{
    public class MainPresenter
    {
        private readonly FolderScanner _folderScanner = new FolderScanner();

        #region View plumbing
        private IMainView _view;

        public IMainView View
        {
            get
            {
                return _view;
            }
            set
            {
                Unsubscribe(_view);
                _view = value;
                Subscribe(_view);
            }
        }

        private void Subscribe(IMainView view)
        {
            if (view == null)
            {
                return;
            }

            view.SelectFolderClick += View_SelectFolderClick;
            view.ScanClick += View_ScanClick;
            view.CancelClick += View_CancelClick;
        }

        private void Unsubscribe(IMainView view)
        {
            if (view == null)
            {
                return;
            }

            view.SelectFolderClick -= View_SelectFolderClick;
            view.ScanClick -= View_ScanClick;
            view.CancelClick -= View_CancelClick;
        }
        #endregion

        private void View_SelectFolderClick(object sender, EventArgs e)
        {
            string folder = View.SelectFolder();
            if (folder != null)
            {
                View.SelectedFolder = folder;
            }
        }

        private void View_ScanClick(object sender, EventArgs e)
        {
            View.EnableUI(false);
            string path = View.SelectedFolder;
            var treeMapSize = View.TreeMapActualSize;
            var treeMapWidth = treeMapSize.Width;
            var treeMapHeight = treeMapSize.Height;
            TreeMapDataSource treeMapDataSource;

            Stopwatch stopwatch = Stopwatch.StartNew();
            Task.Run(() =>
            {
                try
                {
                    _folderScanner.Scan(path);

                    var renderer = new Renderer();
                    var bounds = new RectangleD(0, 0, treeMapWidth, treeMapHeight);
                    treeMapDataSource = renderer.Render(bounds, _folderScanner.TopLevelFolders);
                    stopwatch.Stop();
                    View.RunOnGuiThread(() =>
                    {
                        View.SetResult(_folderScanner, treeMapDataSource,
                        $"Finished scanning in {_folderScanner.Duration}, total time: {stopwatch.Elapsed}");
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

        private void View_CancelClick(object sender, EventArgs e)
        {
            _folderScanner.Cancel();
        }
    }
}
