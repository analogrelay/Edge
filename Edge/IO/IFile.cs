using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Edge.IO
{
    public interface IFile
    {
        string Name { get; }
        string Path { get; }
        string FullPath { get; }
        string Extension { get; }
        bool Exists { get; }

        TextReader OpenRead();
    }
}
