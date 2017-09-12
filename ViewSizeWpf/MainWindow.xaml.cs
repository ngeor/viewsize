using CRLFLabs.ViewSize;
using CRLFLabs.ViewSize.Drawing;
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
    public partial class MainWindow : Window
    {
        private readonly FolderScanner folderScanner;

        public MainWindow()
        {
            InitializeComponent();
            folderScanner = new FolderScanner();
            folderScanner.Scanning += FolderScanner_Scanning;
        }

        private void FolderScanner_Scanning(object sender, FileSystemEventArgs e)
        {
            // TODO: too slow for every file
            //Dispatcher.Invoke(() =>
            //{
            //    lblStatus.Content = e.FileSystemEntry.Path;
            //});
        }

        private void btnSelectFolder_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                var result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    txtFolder.Text = dialog.SelectedPath;
                }
            }
        }

        private void btnScan_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            EnableUI(false);
            string path = txtFolder.Text;
            var treeMapWidth = treeMap.ActualWidth;
            var treeMapHeight = treeMap.ActualHeight;
            TreeMapDataSource treeMapDataSource;

            Stopwatch stopwatch = Stopwatch.StartNew();
            Task.Run(() =>
            {
                try
                {
                    folderScanner.Scan(path);

                    var renderer = new Renderer();
                    var bounds = new RectangleD(0, 0, treeMapWidth, treeMapHeight);
                    treeMapDataSource = renderer.Render(bounds, folderScanner.TopLevelFolders);

                    Dispatcher.Invoke(() =>
                    {
                        treeView.DataContext = null;
                        treeView.DataContext = folderScanner;
                        treeMap.DataSource = treeMapDataSource;

                        stopwatch.Stop();
                        lblStatus.Content = $"Finished scanning in {folderScanner.Duration}, total time: {stopwatch.Elapsed}";
                    });
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show(ex.Message + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    });
                }
                finally
                {
                    Dispatcher.Invoke(() =>
                    {
                        Cursor = Cursors.Arrow;
                        EnableUI(true);
                    });
                }
            });
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            folderScanner.Cancel();
        }

        private void EnableUI(bool enable)
        {
            txtFolder.IsEnabled = enable;
            btnScan.IsEnabled = enable;
            btnSelectFolder.IsEnabled = enable;
            btnCancel.IsEnabled = !enable;
        }

        private void treeMap_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var point = e.GetPosition(treeMap);
            var folder = treeMap.DataSource?.Find(point.ToPointD().Scale(treeMap.ScaleToActual.Invert()));
            if (folder == null)
            {
                MessageBox.Show("No folder at point");
            }
            else
            {
                MessageBox.Show(folder.Folder.Path);
            }
        }
    }
}
