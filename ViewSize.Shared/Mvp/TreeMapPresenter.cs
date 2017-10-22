using System;
using System.ComponentModel;
using CRLFLabs.ViewSize.Drawing;
using CRLFLabs.ViewSize.IO;
using CRLFLabs.ViewSize.TreeMap;

namespace CRLFLabs.ViewSize.Mvp
{
    public class TreeMapPresenter : PresenterBase<ITreeMapView, IMainModel>
    {
        private RectangleD _lastBounds;
        private SortKey _lastSortKey;
        private RectangleD _currentBounds;

        public TreeMapPresenter(ITreeMapView view, IMainModel model)
            : base(view, model)
        {
            Model.PropertyChanging += Model_PropertyChanging;
            Model.PropertyChanged += Model_PropertyChanged;
        }

        protected override void OnViewLoad(object sender, EventArgs e)
        {
            base.OnViewLoad(sender, e);
            View.RedrawNeeded += View_RedrawNeeded;
        }

        void Model_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            switch (e.PropertyName)
            {
                case MainModel.SelectedPropertyName:
                    View.SelectionChanging();
                    break;
            }
        }

        void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case MainModel.SelectedPropertyName:
                    View.SelectionChanged();
                    break;
                case MainModel.SortKeyPropertyName:
                    ReCalculateTreeMap(true);
                    break;
                case MainModel.ChildrenPropertyName:
                    // capture these two for recalculate
                    _lastBounds = View.BoundsD;
                    _lastSortKey = Model.SortKey;
                    View.Redraw();
                    break;
                case MainModel.IsScanningPropertyName:
                    // we are probably about to scan.
                    // capture the current bounds.
                    // this is done on the main thread.
                    _currentBounds = View.BoundsD;
                    break;
                case MainModel.TopLevelFoldersPropertyName:
                    // this is done on a background thread.
                    var renderer = new Renderer(_currentBounds, Model.TopLevelFolders, Model.SortKey);
                    renderer.Render();
                    break;
            }
        }

        void View_RedrawNeeded(object sender, EventArgs e)
        {
            ReCalculateTreeMap(false);
        }

        private void ReCalculateTreeMap(bool forceRedraw)
        {
            if (Model.Children == null)
            {
                // no data yet
                return;
            }

            var bounds = View.BoundsD;
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
                View.Redraw();
            }
        }
    }
}
