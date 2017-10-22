﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppKit;
using CRLFLabs.ViewSize;
using Foundation;
using CRLFLabs.ViewSize.Drawing;
using CRLFLabs.ViewSize.TreeMap;
using CRLFLabs.ViewSize.Mvp;
using CRLFLabs.ViewSize.Settings;
using CRLFLabs.ViewSize.IO;

namespace ViewSizeMac
{
    [Presenter(typeof(MainPresenter))]
    public partial class ViewController : NSViewController, IMainView
    {
        public ViewController(IntPtr handle) : base(handle)
        {
            TroubleshootNativeCrashes();
        }

        private static void TroubleshootNativeCrashes()
        {
#if DEBUG
            new System.Threading.Thread(() =>
            {
                while (true)
                {
                    System.Threading.Thread.Sleep(1000);
                    GC.Collect();
                }
            }).Start();
#endif
        }

        /// <summary>
        /// Called after the view has loaded.
        /// Do any additional setup after loading the view.
        /// </summary>
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // create and assign the outline delegate.
            // since this does not depend on the datasource,
            // it can be done upfront. Also subscribing to the event only once.
            var folderOutlineDelegate = new FolderOutlineDelegate();
            folderOutlineDelegate.SelectionChanged += (sender, e) =>
            {
                var selectedEntry = outlineView.ItemAtRow(outlineView.SelectedRow) as FileSystemEntry;
                OnTreeViewSelectionChanged?.Invoke(this, new FileSystemEventArgs(selectedEntry));
            };
            outlineView.Delegate = folderOutlineDelegate;

            // setup MVP
            PresenterFactory.Create(this);
            Load?.Invoke(this, EventArgs.Empty);

            // we have a model
            Model.PropertyChanged += _mainModel_PropertyChanged;
            folderGraph.Model = Model;
            SetupFolderChooserViewModel();
        }

        public override NSObject RepresentedObject
        {
            get
            {
                return base.RepresentedObject;
            }
            set
            {
                base.RepresentedObject = value;
                // Update the view, if already loaded.
            }
        }

        #region Cocoa Actions

        partial void OnScan(NSObject sender) => OnBeginScanClick?.Invoke(this, EventArgs.Empty);
        partial void OnCancelScan(NSObject sender) => OnCancelScanClick?.Invoke(this, EventArgs.Empty);
        partial void OnShowInFinder(NSObject sender)
        {
            var selected = Model.Selected;
            if (selected == null)
            {
                return;
            }

            NSWorkspace.SharedWorkspace.ActivateFileViewer(new NSUrl[]
            {
                new NSUrl(selected.Path, selected.IsDirectory)
            });
        }
        #endregion

        #region IMainView

        public event EventHandler OnBeginScanClick;
        public event EventHandler OnCancelScanClick;
        public event EventHandler<FileSystemEventArgs> OnTreeViewSelectionChanged;
        public event EventHandler Load;

        public ITreeMapView TreeMapView => folderGraph;
        public string SelectedFolder => txtFolder.StringValue;
        public IMainModel Model { get; set; }

        public void EnableUI(bool enable)
        {
            btnScan.Enabled = enable;
            btnCancel.Enabled = !enable;
            btnSelectFolder.Enabled = enable;
            txtFolder.Enabled = enable;
            if (enable)
            {
                pbScan.StopAnimation(this);
            }
            else
            {
                pbScan.StartAnimation(this);
            }
        }

        public void RunOnGuiThread(Action action) => InvokeOnMainThread(action);

        public void ShowError(Exception ex) => ShowExceptionAlert(ex);

        public void ShowError(string message) => MessageBox.ShowMessage(message);

        void _mainModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == MainModel.ChildrenPropertyName)
            {
                var dataSource = new FolderOutlineDataSource(Model.Children);
                outlineView.DataSource = dataSource;
            }
        }

        public void SetScanningItem(string path) => lblStatus.StringValue = path;

        public void SetDurationLabel(string durationLabel) => lblDuration.StringValue = durationLabel;

        public void SetSelectedTreeViewItem(FileSystemEntry selection)
        {
            FolderOutlineDataSource dataSource = outlineView.DataSource as FolderOutlineDataSource;
            if (dataSource == null)
            {
                return;
            }

            if (selection == null)
            {
                outlineView.DeselectAll(this);
                return;
            }

            foreach (var ancestor in selection.Ancestors())
            {
                outlineView.ExpandItem(ancestor, false);
            }

            var row = outlineView.RowForItem(selection);
            outlineView.SelectRow(row, false);
            outlineView.ScrollRowToVisible(row);
        }

        #endregion

        // used by AppDelegate. TODO: find a better way?
        internal void TriggerSelectFolderClick()
        {
            OnSelectFolder(null);
        }

        [Action("upOneLevel:")]
        public void UpOneLevel(NSObject sender)
        {
            // TODO move to presenter
            var selected = Model.Selected;
            if (selected == null)
            {
                return;
            }

            var parent = selected.Parent as FileSystemEntry;
            if (parent != null)
            {
                Model.Selected = parent;
            }
        }

        private void ShowExceptionAlert(Exception ex)
        {
            MessageBox.ShowMessage(ex.Message + ex.StackTrace, ex.Message);
        }
    }
}
