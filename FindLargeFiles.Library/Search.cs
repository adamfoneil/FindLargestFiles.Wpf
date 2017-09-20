using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FindLargeFiles.Library
{
    public static class Search
    {
        public static async Task<IEnumerable<FileInfo>> FindLargestFilesAsync(string path, int count = 10, IProgress<string> progress = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var files = await FindFilesAsync(path, progress, cancellationToken);

            progress?.Report("Finding largest files...");
            IEnumerable<FileInfo> results = null;
            await Task.Run(() =>
            {
                results = files.OrderByDescending(item => item.Length).Take(count);
            });

            return results;
        }        

        public static async Task<IEnumerable<string>> FindDirectoriesAsync(string path, IProgress<string> progress = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            List<string> results = new List<string>();            
            await Task.Run(() => { FindDirectoriesInner(path, results, progress, cancellationToken); });
            return results;
        }

        public static async Task<IEnumerable<FileInfo>> FindFilesAsync(string path, IProgress<string> progress = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var folders = await FindDirectoriesAsync(path, progress, cancellationToken);

            List<FileInfo> results = new List<FileInfo>();
            await Task.Run(() =>
            {
                foreach (string folder in folders)
                {
                    progress?.Report($"Examining {folder}");
                    string[] files = Directory.GetFiles(folder, "*", SearchOption.TopDirectoryOnly);
                    results.AddRange(files.Select(fileName => new FileInfo(fileName)));
                }
            });

            return results;
        }

        private static void FindDirectoriesInner(string path, List<string> results, IProgress<string> progress = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                if (cancellationToken.IsCancellationRequested) return;

                string[] folders = Directory.GetDirectories(path, "*");

                if (cancellationToken.IsCancellationRequested) return;

                foreach (string folder in folders)
                {
                    progress.Report(folder);
                    results.Add(folder);
                }

                foreach (string folder in folders) FindDirectoriesInner(folder, results, progress, cancellationToken);

            }
            catch
            {
                // do nothing, i.e. Access Denied
            }
        }
    }
}
