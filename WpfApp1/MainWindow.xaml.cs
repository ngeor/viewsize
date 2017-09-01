using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
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
        private BackgroundWorker backgroundWorker;
        private FileEntry rootFileEntry;
        private Progress progressDialog;

        public MainWindow()
        {
            InitializeComponent();

            backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressDialog.progressBar.Maximum = pending;
            progressDialog.progressBar.Value = finished;
            progressDialog.lblProgress.Content = $"Processed {finished} folders out of {pending}";
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            treeView.Items.Clear();
            Populate(treeView.Items, rootFileEntry);
            Cursor = System.Windows.Input.Cursors.Arrow;
            btnScan.IsEnabled = true;
            progressDialog.Close();
        }

        private int pending;
        private int finished;

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            pending = 0;
            finished = 0;
            rootFileEntry.Calculate(x => { pending += x; backgroundWorker.ReportProgress(0); }, x => { finished += x; backgroundWorker.ReportProgress(0); });            
        }

        private void btnSelectFolder_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
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
            Cursor = System.Windows.Input.Cursors.Wait;
            btnScan.IsEnabled = false;
            rootFileEntry = new FileEntry(txtFolder.Text);
            progressDialog = new Progress();
            progressDialog.Show();
            backgroundWorker.RunWorkerAsync();
        }

        private void Populate(ItemCollection items, FileEntry root)
        {
            var treeViewItem = new TreeViewItem
            {
                Header = root.Path + " (" + FileUtils.FormatBytes(root.TotalSize) + $") ({root.Percentage:P2})"
            };

            items.Add(treeViewItem);
            foreach(var child in root.Children.OrderByDescending(c=>c.TotalSize))
            {
                Populate(treeViewItem.Items, child);
            }
        }
    }

    delegate void NotifyProgress(int number);
}
