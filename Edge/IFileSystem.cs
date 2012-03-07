using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Edge
{
    public interface IFileSystem
    {
        bool Exists(string path);
        TextReader OpenRead(string path);
        IEnumerable<string> GetAll(string subroot, string search);
        void EnsureDirectory(string dir);
    }
}
