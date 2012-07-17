using Owin;
using Gate;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using Gate.Middleware;
using System.Diagnostics;
using Edge.IO;
using Edge.Routing;
using Edge.Compilation;
using Edge.Execution;

namespace Edge
{
    public static class AppBuilderExtensions
    {
        public static IAppBuilder UseEdge(this IAppBuilder builder)
        {
            return UseEdge(builder, Environment.CurrentDirectory);
        }

        public static IAppBuilder UseEdge(this IAppBuilder builder, string fileSystemRoot)
        {
            return UseEdge(builder, fileSystemRoot, null);
        }

        public static IAppBuilder UseEdge(this IAppBuilder builder, string fileSystemRoot, string virtualRoot)
        {
            return UseEdge(builder, new PhysicalFileSystem(fileSystemRoot), virtualRoot);
        }

        public static IAppBuilder UseEdge(this IAppBuilder builder, IFileSystem fileSystem)
        {
            return UseEdge(builder, fileSystem, null);
        }

        public static IAppBuilder UseEdge(this IAppBuilder builder, IFileSystem fileSystem, string virtualRoot)
        {
            return UseEdge(builder, new EdgeApplication(
                fileSystem,
                virtualRoot,
                new DefaultRouter(fileSystem),
                new DefaultCompilationManager(),
                new DefaultPageActivator(),
                new DefaultPageExecutor(),
                new GateTraceFactory()));
        }

        public static IAppBuilder UseEdge(this IAppBuilder builder, EdgeApplication app) {
            builder.Use(app.Start);
            return builder;
        }
    }
}

