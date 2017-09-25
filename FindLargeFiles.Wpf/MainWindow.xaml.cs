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
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;

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

            Dictionary<string, BitmapImage> _iconSource = new Dictionary<string, BitmapImage>();

            lbFiles.Items.Clear();
            foreach (var item in largest)
            {
                string iconKey = Path.GetExtension(item.FullName);
                if (iconKey.ToLower().Equals(".exe")) iconKey = Path.GetFileName(item.FullName);
                if (!_iconSource.ContainsKey(iconKey))
                {
                    try
                    {                        
                        var iconBmp = FileSystem.GetIcon(item.FullName, FileSystem.IconSize.Small);
                        BitmapImage imgSrc = ConvertToImageSource(iconBmp);
                        _iconSource.Add(iconKey, imgSrc);                        
                    }
                    catch
                    {
                        // do nothing
                    }                    
                }

                if (_iconSource.ContainsKey(iconKey)) item.Icon = _iconSource[iconKey];
                lbFiles.Items.Add(item);
            }

            _sw.Stop();
            var elapsed = _sw.Elapsed;

            this.Title = $"Find Largest Files - {path}";
            lblStatus.Content = $"Ready - {elapsed.Seconds} seconds";
        }

        private BitmapImage ConvertToImageSource(Bitmap iconBmp)
        {
            // thanks to https://stackoverflow.com/a/1069509/2023653
            using (MemoryStream ms = new MemoryStream())
            {
                iconBmp.Save(ms, ImageFormat.Bmp);
                ms.Position = 0;
                //ms.Seek(0, SeekOrigin.Begin);
                var result = new BitmapImage();
                result.BeginInit();                
                result.StreamSource = ms;
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.EndInit();
                return result;
            }
        }

        private void ShowProgress(string item)
        {
            lblStatus.Content = item;
        }

        private void lbFiles_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                string fileName = ((e.Source as System.Windows.Controls.ListBox).SelectedItem as FileSearchResult).FullName;
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
