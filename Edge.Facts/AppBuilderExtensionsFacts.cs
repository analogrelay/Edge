using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Edge.Compilation;
using Edge.Execution;
using Edge.IO;
using Edge.Routing;
using Gate.Builder;
using Owin;
using VibrantUtils;
using Xunit;

namespace Edge.Facts
{
    public class AppBuilderExtensionsFacts
    {
        private static readonly MethodInfo TheStartMethod = typeof(EdgeApplication).GetMethod("Start", BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new[] { typeof(AppDelegate) }, new ParameterModifier[0]);

        public class TheUseEdgeMethod
        {
            public class WithNoArguments
            {
                [Fact]
                public void RequiresNonNullBuilder()
                {
                    ContractAssert.NotNull(() => AppBuilderExtensions.UseEdge(null, "Foo"), "builder");
                }

                [Fact]
                public void ConfiguresEdgeAppForCurrentDirectoryAtRootVirtualPath()
                {
                    // Arrange
                    TestAppBuilder builder = new TestAppBuilder();

                    // Act
                    builder.UseEdge();

                    // Assert
                    AssertEdgeApplication(builder.MiddlewareStack.Single());
                }
            }

            public class WithRootDirectoryArgument
            {
                [Fact]
                public void RequiresNonNullBuilder()
                {
                    ContractAssert.NotNull(() => AppBuilderExtensions.UseEdge(null, "Foo"), "builder");
                }

                [Fact]
                public void RequiresNonNullOrEmptyRootDirectory()
                {
                    ContractAssert.NotNullOrEmpty(s => AppBuilderExtensions.UseEdge(new TestAppBuilder(), s), "rootDirectory");
                }

                [Fact]
                public void ConfiguresEdgeAppForSpecifiedDirectoryAtRootVirtualPath()
                {
                    // Arrange
                    TestAppBuilder builder = new TestAppBuilder();

                    // Act
                    builder.UseEdge("Foo");

                    // Assert
                    AssertEdgeApplication(builder.MiddlewareStack.Single(), "/", new PhysicalFileSystem("Foo"));
                }
            }

            public class WithRootDirectoryAndVirtualRootArguments
            {
                [Fact]
                public void RequiresNonNullBuilder()
                {
                    ContractAssert.NotNull(() => AppBuilderExtensions.UseEdge(null, "Foo", "/Bar"), "builder");
                }

                [Fact]
                public void RequiresNonNullOrEmptyRootDirectory()
                {
                    ContractAssert.NotNullOrEmpty(s => AppBuilderExtensions.UseEdge(new TestAppBuilder(), s, "/Bar"), "rootDirectory");
                }

                [Fact]
                public void RequiresNonNullOrEmptyVirtualRoot()
                {
                    ContractAssert.NotNullOrEmpty(s => AppBuilderExtensions.UseEdge(new TestAppBuilder(), "Foo", s), "virtualRoot");
                }

                [Fact]
                public void ConfiguresEdgeAppForSpecifiedDirectoryAtSpecifiedVirtualPath()
                {
                    // Arrange
                    TestAppBuilder builder = new TestAppBuilder();

                    // Act
                    builder.UseEdge("Foo", "Bar");

                    // Assert
                    AssertEdgeApplication(builder.MiddlewareStack.Single(), "Bar", new PhysicalFileSystem("Foo"));
                }
            }

            public class WithFileSystemArgument
            {
                [Fact]
                public void RequiresNonNullBuilder()
                {
                    ContractAssert.NotNull(() => AppBuilderExtensions.UseEdge(null, new PhysicalFileSystem(@"C:\")), "builder");
                }

                [Fact]
                public void RequiresNonNullOrEmptyFileSystem()
                {
                    ContractAssert.NotNull(() => AppBuilderExtensions.UseEdge(new TestAppBuilder(), (IFileSystem)null), "fileSystem");
                }

                [Fact]
                public void ConfiguresEdgeAppForSpecifiedFileSystemAtRootVirtualPath()
                {
                    // Arrange
                    TestAppBuilder builder = new TestAppBuilder();

                    // Act
                    builder.UseEdge(new PhysicalFileSystem(@"C:\Blarg"));

                    // Assert
                    AssertEdgeApplication(builder.MiddlewareStack.Single(), "/", new PhysicalFileSystem(@"C:\Blarg"));
                }
            }

            public class WithFileSystemAndVirtualRootArguments
            {
                [Fact]
                public void RequiresNonNullBuilder()
                {
                    ContractAssert.NotNull(() => AppBuilderExtensions.UseEdge(null, new PhysicalFileSystem(@"C:\")), "builder");
                }

                [Fact]
                public void RequiresNonNullOrEmptyFileSystem()
                {
                    ContractAssert.NotNull(() => AppBuilderExtensions.UseEdge(new TestAppBuilder(), (IFileSystem)null), "fileSystem");
                }

                [Fact]
                public void RequiresNonNullOrEmptyVirtualRoot()
                {
                    ContractAssert.NotNullOrEmpty(s => AppBuilderExtensions.UseEdge(new TestAppBuilder(), new PhysicalFileSystem(@"C:\"), s), "virtualRoot");
                }

                [Fact]
                public void ConfiguresEdgeAppForSpecifiedFileSystemAtSpecifiedVirtualPath()
                {
                    // Arrange
                    TestAppBuilder builder = new TestAppBuilder();

                    // Act
                    builder.UseEdge(new PhysicalFileSystem(@"C:\Blarg"), "Bar");

                    // Assert
                    AssertEdgeApplication(builder.MiddlewareStack.Single(), "Bar", new PhysicalFileSystem(@"C:\Blarg"));
                }
            }

            public class WithEdgeApplicationArgument
            {
                [Fact]
                public void RequiresNonNullBuilder()
                {
                    ContractAssert.NotNull(() => AppBuilderExtensions.UseEdge(null, CreateEdgeApp()), "builder");
                }

                [Fact]
                public void RequiresNonNullApp()
                {
                    ContractAssert.NotNull(() => AppBuilderExtensions.UseEdge(new AppBuilder(), (EdgeApplication)null), "app");
                }

                [Fact]
                public void ConfiguresSpecifiedEdgeApp()
                {
                    // Arrange
                    TestAppBuilder builder = new TestAppBuilder();
                    var app = CreateEdgeApp();

                    // Act
                    builder.UseEdge(app);

                    // Assert
                    Delegate del = builder.MiddlewareStack.Single();
                    Assert.Equal(TheStartMethod, del.Method);
                    Assert.Same(app, del.Target);
                }

                private EdgeApplication CreateEdgeApp()
                {
                    var fs = new PhysicalFileSystem(@"C:\");
                    return new EdgeApplication(
                        fs,
                        "/",
                        new DefaultRouter(fs),
                        new DefaultCompilationManager(new TimestampContentIdentifier()),
                        new DefaultPageActivator(),
                        new DefaultPageExecutor(),
                        new GateTraceFactory());
                }
            }
        }

        public static void AssertEdgeApplication(Delegate del)
        {
            AssertEdgeApplication(del, "/");
        }

        public static void AssertEdgeApplication(Delegate del, string virtualPath)
        {
            AssertEdgeApplication(del, "/", new PhysicalFileSystem(Environment.CurrentDirectory));
        }

        public static void AssertEdgeApplication(Delegate del, string virtualPath, IFileSystem expectedFs)
        {
            EdgeApplication app = del.Target as EdgeApplication;
            Assert.NotNull(app);
            Assert.Equal(virtualPath, app.VirtualRoot);
            Assert.Equal(expectedFs, app.FileSystem);
            Assert.Equal(del.Method, TheStartMethod);
        }
    }
}
