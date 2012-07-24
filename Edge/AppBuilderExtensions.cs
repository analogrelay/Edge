using Owin;
using Gate;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Edge.IO;
using Edge.Routing;
using Edge.Compilation;
using Edge.Execution;
using VibrantUtils;

namespace Edge
{
    public static class AppBuilderExtensions
    {
        public static IAppBuilder UseEdge(this IAppBuilder builder)
        {
            Requires.NotNull(builder, "builder");

            return UseEdge(builder, new PhysicalFileSystem(Environment.CurrentDirectory), "/");
        }

        public static IAppBuilder UseEdge(this IAppBuilder builder, string rootDirectory)
        {
            Requires.NotNull(builder, "builder");
            Requires.NotNullOrEmpty(rootDirectory, "rootDirectory");

            return UseEdge(builder, new PhysicalFileSystem(rootDirectory), "/");
        }

        public static IAppBuilder UseEdge(this IAppBuilder builder, string rootDirectory, string virtualRoot)
        {
            Requires.NotNull(builder, "builder");
            Requires.NotNullOrEmpty(rootDirectory, "rootDirectory");
            Requires.NotNullOrEmpty(virtualRoot, "virtualRoot");

            return UseEdge(builder, new PhysicalFileSystem(rootDirectory), virtualRoot);
        }

        public static IAppBuilder UseEdge(this IAppBuilder builder, IFileSystem fileSystem)
        {
            Requires.NotNull(builder, "builder");
            Requires.NotNull(fileSystem, "fileSystem");
            
            return UseEdge(builder, fileSystem, "/");
        }

        public static IAppBuilder UseEdge(this IAppBuilder builder, IFileSystem fileSystem, string virtualRoot)
        {
            Requires.NotNull(builder, "builder");
            Requires.NotNull(fileSystem, "fileSystem");
            Requires.NotNullOrEmpty(virtualRoot, "virtualRoot");
            
            return UseEdge(builder, new EdgeApplication(
                fileSystem,
                virtualRoot,
                new DefaultRouter(fileSystem),
                new DefaultCompilationManager(new TimestampContentIdentifier()),
                new DefaultPageActivator(),
                new DefaultPageExecutor(),
                new GateTraceFactory()));
        }

        public static IAppBuilder UseEdge(this IAppBuilder builder, EdgeApplication app) {
            Requires.NotNull(builder, "builder");
            Requires.NotNull(app, "app");
            
            builder.Use(app.Start);
            return builder;
        }
    }
}

