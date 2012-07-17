using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edge.IO;

namespace Edge.Compilation
{
    public class DefaultCompilationManager : ICompilationManager
    {
        private IList<ICompiler> _compilers = new List<ICompiler>() {
            new RazorCompiler()
        };

        public void RegisterCompiler(ICompiler compiler)
        {
            _compilers.Add(compiler);
        }

        public async Task<CompilationResult> Compile(IFile file)
        {
            foreach (ICompiler compiler in _compilers)
            {
                if (compiler.CanCompile(file))
                {
                    return await compiler.Compile(file);
                }
            }
            return CompilationResult.Failed(new [] {
                new CompilationMessage(MessageLevel.Error, "Cannot find a suitable compiler for this file", new FileLocation(file.FullPath))
            });
        }
    }
}
