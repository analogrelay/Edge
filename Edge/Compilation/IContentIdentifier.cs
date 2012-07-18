using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edge.IO;

namespace Edge.Compilation
{
    public interface IContentIdentifier
    {
        string GenerateContentId(IFile file);
    }
}
