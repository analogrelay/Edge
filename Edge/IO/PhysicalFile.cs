using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Edge.IO
{
    public class PhysicalFile : IFile
    {
        private FileInfo _fileInfo;

        public string Path { get; private set; }
        public string FullPath { get; private set; }

        public bool Exists
        {
            get { return _fileInfo.Exists; }
        }

        public string Extension
        {
            get { return _fileInfo.Extension; }
        }

        public string Name
        {
            get { return System.IO.Path.GetFileNameWithoutExtension(_fileInfo.Name); }
        }

        public DateTime LastModifiedTime
        {
            get { return _fileInfo.LastWriteTime.ToUniversalTime(); }
        }

        public PhysicalFile(string root, string relativePath)
        {
            Path = relativePath.Replace("/", System.IO.Path.DirectorySeparatorChar.ToString());
            FullPath = System.IO.Path.Combine(root, Path);
            _fileInfo = new FileInfo(FullPath);
        }

        public TextReader OpenRead()
        {
            return _fileInfo.OpenText();
        }
    }
}
