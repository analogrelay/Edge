using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edge.IO
{
    public class PhysicalFileSystem : IFileSystem
    {
        public string Root { get; private set; }

        public PhysicalFileSystem(string root)
        {
            Root = root;
        }

        public IFile GetFile(string path)
        {
            return new PhysicalFile(Root, path);
        }
    }
}
