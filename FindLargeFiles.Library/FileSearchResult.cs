using AdamOneilSoftware;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace FindLargeFiles.Library
{
    public class FileSearchResult
    {
        private readonly string _fullName;
        private readonly DateTime _lastWritten;
        private readonly long _length;
        private readonly ImageSource _icon;

        private static Dictionary<string, ImageSource> _iconSource = new Dictionary<string, ImageSource>();

        public FileSearchResult(string fileName)
        {
            FileInfo fi = new FileInfo(fileName);
            _fullName = fileName;
            _lastWritten = fi.LastWriteTime;
            _length = fi.Length;

            string ext = Path.GetExtension(fileName);
            if (!_iconSource.ContainsKey(ext))
            {
                var icon = FileSystem.GetIcon(fileName, FileSystem.IconSize.Small);
                
                
                //_iconSource.Add(ext, );
            }
            //_icon = _iconSource[ext];
        }

        public string FullName { get { return _fullName; } }
        public DateTime DateModified { get { return _lastWritten; } }
        public long Length { get { return _length; } }
        public ImageSource Icon { get { return _icon; } }
    }
}
