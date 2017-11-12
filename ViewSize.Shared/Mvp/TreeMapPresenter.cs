// <copyright file="TreeMapPresenter.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using CRLFLabs.ViewSize.Drawing;
using CRLFLabs.ViewSize.IO;
using CRLFLabs.ViewSize.TreeMap;

namespace CRLFLabs.ViewSize.Mvp
{
    public class TreeMapPresenter : PresenterBase<ITreeMapView, IMainModel>
    {
        private RectangleD lastBounds;
        private SortKey lastSortKey;
        private RectangleD currentBounds;

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

        private void Model_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            switch (e.PropertyName)
            {
                case MainModel.SelectedPropertyName:
                    View.SelectionChanging();
                    break;
            }
        }

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
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
                    lastBounds = View.BoundsD;
                    lastSortKey = Model.SortKey;
                    View.Redraw();
                    break;
                case MainModel.IsScanningPropertyName:
                    // we are probably about to scan.
                    // capture the current bounds.
                    // this is done on the main thread.
                    currentBounds = View.BoundsD;
                    break;
                case MainModel.TopLevelFoldersPropertyName:
                    // this is done on a background thread.
                    var renderer = new Renderer(currentBounds, Model.TopLevelFolders, Model.SortKey);
                    renderer.Render();
                    break;
            }
        }

        private void View_RedrawNeeded(object sender, EventArgs e)
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
            if (bounds.Size == lastBounds.Size && Model.SortKey == lastSortKey)
            {
                return;
            }

            lastBounds = bounds;
            lastSortKey = Model.SortKey;

            var renderer = new Renderer(bounds, Model.Children, Model.SortKey);
            renderer.Render();

            if (forceRedraw)
            {
                View.Redraw();
            }
        }
    }
}
