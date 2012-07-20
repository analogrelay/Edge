using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edge.IO;
using VibrantUtils;

namespace Edge.Compilation
{
    public class DefaultCompilationManager : ICompilationManager
    {
        private IList<ICompiler> _compilers = new List<ICompiler>() {
            new RazorCompiler()
        };

        public IContentIdentifier ContentIdentifier { get; protected set; }
        public IList<ICompiler> Compilers
        {
            get { return _compilers; }
        }

        internal IDictionary<string, WeakReference<Type>> Cache { get; private set; }

        protected DefaultCompilationManager() {
            Cache = new Dictionary<string, WeakReference<Type>>();
        }
        
        public DefaultCompilationManager(IContentIdentifier identifier) : this()
        {
            Requires.NotNull(identifier, "identifier");

            ContentIdentifier = identifier;
        }

        public Task<CompilationResult> Compile(IFile file, ITrace tracer)
        {
            Requires.NotNull(file, "file");
            Requires.NotNull(tracer, "tracer");

            // Generate a content id
            string contentId = ContentIdentifier.GenerateContentId(file);
            tracer.WriteLine("CompilationManager - Content ID: {0}", contentId);

            WeakReference<Type> cacheEntry;
            if (Cache.TryGetValue(contentId, out cacheEntry))
            {
                Type cached;
                if (cacheEntry.TryGetTarget(out cached))
                {
                    return Task.FromResult(CompilationResult.FromCache(cached));
                }
                else
                {
                    tracer.WriteLine("CompilationManager - Expired: {0}", contentId);
                    Cache.Remove(contentId);
                }
            }

            foreach (ICompiler compiler in _compilers)
            {
                if (compiler.CanCompile(file))
                {
                    tracer.WriteLine("CompilationManager - Selected compiler: '{0}'", compiler.GetType().Name);
                    return CompileWith(compiler, contentId, file);
                }
            }

            return Task.FromResult(CompilationResult.Failed(null, new [] {
                new CompilationMessage(
                    MessageLevel.Error, 
                    Strings.DefaultCompilationManager_CannotFindCompiler, 
                    new FileLocation(file.FullPath))
            }));
        }

        private async Task<CompilationResult> CompileWith(ICompiler compiler, string contentId, IFile file)
        {
            CompilationResult result = await compiler.Compile(file);
            if (result.Success)
            {
                Cache[contentId] = new WeakReference<Type>(result.GetCompiledType());
            }
            return result;
        }
    }
}
