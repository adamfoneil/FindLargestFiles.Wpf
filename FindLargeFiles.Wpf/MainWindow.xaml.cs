using FindLargeFiles.Library;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Forms;
using System.IO;
using AdamOneilSoftware;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows.Media;

namespace FindLargeFiles.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Stopwatch _sw = null;
        private string _path = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void btnFindFolders_Click(object sender, RoutedEventArgs e)
        {
            await PromptForFolder();
        }

        private async Task PromptForFolder()
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();            
            if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

            btnFindFolders.IsEnabled = false;
            btnFindFolders.Content = "Working, please wait...";
            _path = dlg.SelectedPath;
            await FindLargestFiles(dlg.SelectedPath);

            btnFindFolders.Content = "Find 10 Largest Files in Folder...";
            btnFindFolders.IsEnabled = true;
        }

        private async Task FindLargestFiles(string path)
        {
            IProgress<string> progress = new Progress<string>(ShowProgress);

            _sw = Stopwatch.StartNew();

            var largest = await Search.FindLargestFilesAsync(path, progress: progress);

            Dictionary<string, ImageSource> _iconSource = new Dictionary<string, ImageSource>();

            lbFiles.Items.Clear();
            foreach (var item in largest)
            {
                string ext = Path.GetExtension(item.FullName);
                if (!_iconSource.ContainsKey(ext))
                {
                    try
                    {                        
                        var icon = FileSystem.GetIcon(item.FullName, FileSystem.IconSize.Small);
                        //_iconSource.Add(ext, );
                        //_icon = _iconSource[ext];
                    }
                    catch
                    {
                        // do nothing, something wrong with icon retrieval
                    }
                }

                lbFiles.Items.Add(item);
            }

            _sw.Stop();
            var elapsed = _sw.Elapsed;

            this.Title = $"Find Larges Files - {path}";
            lblStatus.Content = $"Ready - {elapsed.Seconds} seconds";
        }

        private void ShowProgress(string item)
        {
            lblStatus.Content = item;
        }

        private void lbFiles_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                string fileName = ((e.Source as System.Windows.Controls.ListBox).SelectedItem as FileInfo).FullName;
                if (fileName != null) Shell.ViewFileLocation(fileName);
            }
            catch (Exception exc)
            {
                System.Windows.MessageBox.Show(exc.Message);
            }
        }

        private async void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.F5 && Directory.Exists(_path))
            {
                await FindLargestFiles(_path);
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await PromptForFolder();
        }
    }
}
