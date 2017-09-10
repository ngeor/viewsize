﻿using CRLFLabs.ViewSize;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ViewSizeWpf.Controls;

namespace WpfApp1
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
            var treeMapDataSource = new TreeMapDataSource
            {
                FoldersWithDrawSize = new List<CRLFLabs.ViewSize.TreeMap.FolderWithDrawSize>(),
                ActualHeight = treeMapHeight,
                ActualWidth = treeMapWidth
            };

            Stopwatch stopwatch = Stopwatch.StartNew();
            Task.Run(() =>
            {
                try
                {
                    folderScanner.Scan(path);

                    var renderer = new CRLFLabs.ViewSize.TreeMap.Renderer
                    {
                        DoRender = (r) => treeMapDataSource.FoldersWithDrawSize.Add(r)
                    };

                    var bounds = new CRLFLabs.ViewSize.Drawing.RectangleF(0, 0, treeMapWidth, treeMapHeight);
                    renderer.Render(bounds, folderScanner.TopLevelFolders);

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
    }
}
