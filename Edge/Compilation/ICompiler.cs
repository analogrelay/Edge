using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edge.IO;

namespace Edge.Compilation
{
    public interface ICompiler
    {
        bool CanCompile(IFile file);
        Task<CompilationResult> Compile(IFile file);
    }
}
