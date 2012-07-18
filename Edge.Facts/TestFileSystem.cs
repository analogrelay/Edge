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
        private Dictionary<string, IFile> _testFiles = new Dictionary<string, IFile>();

        public string Root { get; private set; }

        public TestFileSystem(string root)
        {
            Root = root;
        }

        public IFile GetFile(string path)
        {
            IFile file;
            if (!_testFiles.TryGetValue(path, out file))
            {
                return new TestFile(Path.Combine(Root, path), path);
            }
            return file;
        }

        public IFile AddTestFile(string path, string content)
        {
            var file = new TestFile(Path.Combine(Root, path), path, content);
            _testFiles.Add(path, file);
            return file;
        }
    }
}
