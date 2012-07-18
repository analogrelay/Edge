using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Edge.IO;

namespace Edge.Facts
{
    class TestFile : IFile
    {
        public string Name { get; private set; }
        public string Path { get; private set; }
        public string FullPath { get; private set; }
        public string Extension { get; private set; }
        public bool Exists { get; private set; }
        public string TextContent { get; private set; }
        
        public TestFile(string fullPath, string path)
        {
            FullPath = fullPath;
            Path = path;
            Exists = false;
            Name = System.IO.Path.GetFileNameWithoutExtension(path);
            Extension = System.IO.Path.GetExtension(path);
            TextContent = String.Empty;
        }

        public TestFile(string fullPath, string path, string textContent) : this(fullPath, path)
        {
            TextContent = textContent;
            Exists = true;
        }

        public TextReader OpenRead()
        {
            return new StringReader(TextContent);
        }
    }
}
