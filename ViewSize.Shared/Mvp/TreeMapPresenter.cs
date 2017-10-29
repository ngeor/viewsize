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
            this.Model.PropertyChanging += this.Model_PropertyChanging;
            this.Model.PropertyChanged += this.Model_PropertyChanged;
        }

        protected override void OnViewLoad(object sender, EventArgs e)
        {
            base.OnViewLoad(sender, e);
            this.View.RedrawNeeded += this.View_RedrawNeeded;
        }

        private void Model_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            switch (e.PropertyName)
            {
                case MainModel.SelectedPropertyName:
                    this.View.SelectionChanging();
                    break;
            }
        }

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case MainModel.SelectedPropertyName:
                    this.View.SelectionChanged();
                    break;
                case MainModel.SortKeyPropertyName:
                    this.ReCalculateTreeMap(true);
                    break;
                case MainModel.ChildrenPropertyName:
                    // capture these two for recalculate
                    this.lastBounds = this.View.BoundsD;
                    this.lastSortKey = this.Model.SortKey;
                    this.View.Redraw();
                    break;
                case MainModel.IsScanningPropertyName:
                    // we are probably about to scan.
                    // capture the current bounds.
                    // this is done on the main thread.
                    this.currentBounds = this.View.BoundsD;
                    break;
                case MainModel.TopLevelFoldersPropertyName:
                    // this is done on a background thread.
                    var renderer = new Renderer(this.currentBounds, this.Model.TopLevelFolders, this.Model.SortKey);
                    renderer.Render();
                    break;
            }
        }

        private void View_RedrawNeeded(object sender, EventArgs e)
        {
            this.ReCalculateTreeMap(false);
        }

        private void ReCalculateTreeMap(bool forceRedraw)
        {
            if (this.Model.Children == null)
            {
                // no data yet
                return;
            }

            var bounds = this.View.BoundsD;
            if (bounds.Size == this.lastBounds.Size && this.Model.SortKey == this.lastSortKey)
            {
                return;
            }

            this.lastBounds = bounds;
            this.lastSortKey = this.Model.SortKey;

            var renderer = new Renderer(bounds, this.Model.Children, this.Model.SortKey);
            renderer.Render();

            if (forceRedraw)
            {
                this.View.Redraw();
            }
        }
    }
}
