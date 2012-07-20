using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Edge.Compilation;
using Edge.Execution;
using Edge.IO;
using Edge.Routing;
using Gate;
using Owin;
using VibrantUtils;

namespace Edge
{
    public class EdgeApplication
    {
        public IFileSystem FileSystem { get; protected set; }
        public string VirtualRoot { get; protected set; }
        public IRouter Router { get; protected set; }
        public ICompilationManager CompilationManager { get; protected set; }
        public IPageExecutor Executor { get; protected set; }
        public IPageActivator Activator { get; protected set; }
        public ITraceFactory Tracer { get; protected set; }

        // Consumers should use IoC or the Default UseEdge extension method to initialize this.
        public EdgeApplication(
            IFileSystem fileSystem,
            string virtualRoot,
            IRouter router,
            ICompilationManager compiler,
            IPageActivator activator,
            IPageExecutor executor,
            ITraceFactory tracer)
            : this()
        {
            Requires.NotNull(fileSystem, "fileSystem");
            Requires.NotNullOrEmpty(virtualRoot, "virtualRoot");
            Requires.NotNull(router, "router");
            Requires.NotNull(compiler, "compiler");
            Requires.NotNull(activator, "activator");
            Requires.NotNull(executor, "executor");
            Requires.NotNull(tracer, "tracer");

            FileSystem = fileSystem;
            VirtualRoot = virtualRoot;
            Router = router;
            CompilationManager = compiler;
            Executor = executor;
            Activator = activator;
            Tracer = tracer;
        }

        /// <summary>
        /// Use at your OWN RISK. NOTHING will be initialized for you!
        /// </summary>
        protected EdgeApplication()
        {
        }

        public AppDelegate Start()
        {
            return Start(new AppDelegate(_ => Task.FromResult(new ResultParameters())));
        }

        public AppDelegate Start(AppDelegate next)
        {
            var global = Tracer.ForApplication();
            global.WriteLine("Started at '{0}'=>'{1}'", VirtualRoot, FileSystem.Root);
            return async call =>
            {
                Stopwatch sw = new Stopwatch();
                Request req = new Request(call);
                var trace = Tracer.ForRequest(req);
                using (trace.StartTrace())
                {
                    trace.WriteLine("Recieved {0} {1}", req.Method, req.Path);

                    if (!IsUnder(VirtualRoot, req.Path))
                    {
                        // Not for us!
                        return await next(call);
                    }

                    // Step 1. Route the request to a file
                    RouteResult routed = await Router.Route(req, trace);
                    if (!routed.Success)
                    {
                        // Also not for us!
                        return await next(call);
                    }
                    trace.WriteLine("Router: '{0}' ==> '{1}'::'{2}'", req.Path, routed.File.Path, routed.PathInfo);

                    // Step 2. Use the compilation manager to get the file's compiled type
                    sw.Start();
                    CompilationResult compiled = await CompilationManager.Compile(routed.File, trace);
                    sw.Stop();
                    if (!compiled.Success)
                    {
                        trace.WriteLine("Compiler: '{0}' FAILED", routed.File.Name);
                        throw new CompilationFailedException(compiled.Messages);
                    }
                    if (compiled.SatisfiedFromCache)
                    {
                        trace.WriteLine("Retrieved compiled code from cache in {0}ms", sw.ElapsedMilliseconds);
                    }
                    else
                    {
                        trace.WriteLine("Compiled '{0}' in {1}ms", routed.File.Path, sw.ElapsedMilliseconds);
                    }
                    sw.Reset();

                    // Step 3. Construct an instance using the PageActivator
                    Type type = compiled.GetCompiledType();
                    ActivationResult activated = Activator.ActivatePage(type, trace);
                    if (!activated.Success)
                    {
                        trace.WriteLine("Activator: '{0}' FAILED", type.FullName);
                        throw new ActivationFailedException(type);
                    }
                    trace.WriteLine("Activator: '{0}' SUCCESS", type.FullName);

                    // Step 4. Execute the activated instance!
                    Response resp = await Executor.Execute(activated.Page, req, trace);
                    return await resp.GetResultAsync();
                }
            };
        }

        internal static bool IsUnder(string root, string path)
        {
            if (String.IsNullOrEmpty(root))
            {
                return true;
            }
            root = root.TrimEnd('/');
            return path.StartsWith(root + "/");
        }
    }
}
