using AdamOneilSoftware;
using FindLargeFiles.Library;
using System;
using System.Collections.Generic;
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

namespace FindLargeFiles.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Stopwatch _sw = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void btnFindFolders_Click(object sender, RoutedEventArgs e)
        {
            var title = this.Title;
            this.Title = "Searching...";
            IProgress<string> progress = new Progress<string>(ShowProgress);

            _sw = Stopwatch.StartNew();

            var largest = await Search.FindLargestFilesAsync(@"C:\Users\Adam\Dropbox", progress: progress);            
            
            foreach (var item in largest) lbFiles.Items.Add(item);
            
            _sw.Stop();
            var elapsed = _sw.Elapsed;

            this.Title = title;
            lblStatus.Content = $"Ready - last run {elapsed.Seconds} seconds";
        }

        private void ShowProgress(string item)
        {
            lblStatus.Content = item;
        }
    }
}
