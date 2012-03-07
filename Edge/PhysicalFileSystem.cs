using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Edge
{
    public class PhysicalFileSystem : IFileSystem
    {
        public string Root { get; private set; }

        public PhysicalFileSystem(string root)
        {
            Root = root;
        }

        public bool Exists(string path)
        {
            return File.Exists(Path.Combine(Root, path));
        }


        public TextReader OpenRead(string path)
        {
            return File.OpenText(Path.Combine(Root, path));
        }


        public IEnumerable<string> GetAll(string subpath, string search)
        {
            return Directory.EnumerateFiles(
                Path.Combine(Root, subpath),
                search);
        }


        public void EnsureDirectory(string dir)
        {
            string fullDir = Path.Combine(Root, dir);
            if (!Directory.Exists(fullDir))
            {
                Directory.CreateDirectory(fullDir);
            }
        }
    }
}
