using CRLFLabs.ViewSize;
using System;
using System.ComponentModel;
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
            Dispatcher.Invoke(() =>
            {
                lblStatus.Content = e.FileSystemEntry.Path;
            });
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

            Task.Run(() =>
            {
                try
                {
                    folderScanner.Scan(path);
                    Dispatcher.Invoke(() =>
                    {
                        //treeView.Items.Clear();
                        //Populate(treeView.Items, folderScanner.Root);
                        treeView.DataContext = null;
                        treeView.DataContext = folderScanner;
                        treeMap.DataSource = folderScanner.TopLevelFolders;
                        lblStatus.Content = $"Finished in {folderScanner.Duration}";
                    });
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void Populate(ItemCollection items, IFileSystemEntry folder)
        {
            var treeViewItem = new TreeViewItem
            {
                Header = folder.DisplayText + " (" + folder.DisplaySize + ")"
            };

            items.Add(treeViewItem);
            foreach (var child in folder.Children.OrderByDescending(c => c.TotalSize))
            {
                Populate(treeViewItem.Items, child);
            }
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
