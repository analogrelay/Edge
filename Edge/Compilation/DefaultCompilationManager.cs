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
        private Dictionary<string, WeakReference<Type>> _cache = new Dictionary<string, WeakReference<Type>>();

        private IList<ICompiler> _compilers = new List<ICompiler>() {
            new RazorCompiler()
        };

        public IContentIdentifier ContentIdentifier { get; private set; }

        public DefaultCompilationManager(IContentIdentifier identifier)
        {
            ContentIdentifier = identifier;
        }

        public void RegisterCompiler(ICompiler compiler)
        {
            _compilers.Add(compiler);
        }

        public async Task<CompilationResult> Compile(IFile file, ITrace tracer)
        {
            // Generate a content id
            string contentId = ContentIdentifier.GenerateContentId(file);
            tracer.WriteLine("CompilationManager - Content ID: {0}", contentId);

            WeakReference<Type> cached;
            Type compiled;
            if (_cache.TryGetValue(contentId, out cached) && cached.TryGetTarget(out compiled))
            {
                return CompilationResult.FromCache(compiled);
            }

            foreach (ICompiler compiler in _compilers)
            {
                if (compiler.CanCompile(file))
                {
                    tracer.WriteLine("CompilationManager - Selected compiler: '{0}'", compiler.GetType().Name);
                    CompilationResult result = await compiler.Compile(file);
                    if (result.Success)
                    {
                        _cache[contentId] = new WeakReference<Type>(result.GetCompiledType());
                    }
                    return result;
                }
            }
            return CompilationResult.Failed(new [] {
                new CompilationMessage(MessageLevel.Error, "Cannot find a suitable compiler for this file", new FileLocation(file.FullPath))
            });
        }
    }
}
