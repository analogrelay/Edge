using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Edge.IO;

namespace Edge.Facts
{
    class TestFileSystem : IFileSystem
    {
        public string Root { get; private set; }

        public TestFileSystem(string root)
        {
            Root = root;
        }

        public IFile GetFile(string path)
        {
            return new TestFile(Path.Combine(Root, path), path, exists: false);
        }
    }
}
