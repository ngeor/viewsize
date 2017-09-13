using CRLFLabs.ViewSize;
using CRLFLabs.ViewSize.Drawing;
using CRLFLabs.ViewSize.Mvp;
using CRLFLabs.ViewSize.TreeMap;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ViewSizeWpf;
using ViewSizeWpf.Controls;

namespace ViewSizeWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMainView
    {
        private readonly MainPresenter mainPresenter = new MainPresenter();

        public MainWindow()
        {
            InitializeComponent();
            mainPresenter.View = this;
        }

        private void treeMap_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var point = e.GetPosition(treeMap);
            var folder = treeMap.DataSource?.Find(point.ToPointD().Scale(treeMap.ScaleToActual.Invert()));
            treeMap.Selected = folder;
        }

        private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var fileSystemEntry = treeView.SelectedItem as FileSystemEntry;
            if (fileSystemEntry == null)
            {
                return;
            }

            treeMap.Selected = treeMap.DataSource.Find(fileSystemEntry.Path);
        }

        #region IMainView implementation
        public string SelectedFolder
        {
            get { return txtFolder.Text; }
            set { txtFolder.Text = value; }
        }

        public event EventHandler SelectFolderClick
        {
            add
            {
                btnSelectFolder.Click += ToRoutedEventHandler(value);
            }
            remove
            {
                btnSelectFolder.Click -= ToRoutedEventHandler(value);
            }
        }

        public string SelectFolder()
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                var result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    return dialog.SelectedPath;
                }
            }

            return null;
        }

        public event EventHandler ScanClick
        {
            add
            {
                btnScan.Click += ToRoutedEventHandler(value);
            }

            remove
            {
                btnScan.Click -= ToRoutedEventHandler(value);
            }
        }

        public event EventHandler CancelClick
        {
            add
            {
                btnCancel.Click += ToRoutedEventHandler(value);
            }

            remove
            {
                btnCancel.Click -= ToRoutedEventHandler(value);
            }
        }

        public void EnableUI(bool enable)
        {
            txtFolder.IsEnabled = enable;
            btnScan.IsEnabled = enable;
            btnSelectFolder.IsEnabled = enable;
            btnCancel.IsEnabled = !enable;
            Cursor = enable ? Cursors.Arrow : Cursors.Wait;
        }

        public SizeD TreeMapActualSize => treeMap.ActualSize;

        public void RunOnGuiThread(Action action) => Dispatcher.Invoke(action);

        public void ShowError(Exception ex)
        {
            MessageBox.Show(ex.Message + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void SetResult(FolderScanner folderScanner, TreeMapDataSource treeMapDataSource, string durationLabel)
        {
            treeView.DataContext = null;
            treeView.DataContext = folderScanner;
            treeMap.DataSource = treeMapDataSource;

            lblStatus.Content = durationLabel;
        }
        #endregion

        private static RoutedEventHandler ToRoutedEventHandler(EventHandler eventHandler)
        {
            return (obj, e) => eventHandler(obj, e);
        }
    }
}
