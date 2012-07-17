using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edge.IO
{
    public interface IFileSystem
    {
        string Root { get; }
        IFile GetFile(string path);
    }
}
