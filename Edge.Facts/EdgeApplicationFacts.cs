using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edge.Compilation;
using Edge.Execution;
using Edge.IO;
using Edge.Routing;
using Moq;
using VibrantUtils;
using Xunit;

namespace Edge.Facts
{
    public class EdgeApplicationFacts
    {
        public class TheConstructor
        {
            [Fact]
            public void RequiresNonNullFileSystem()
            {
                ContractAssert.NotNull(() => new EdgeApplication(
                    null,
                    "Foo",
                    new Mock<IRouter>().Object,
                    new Mock<ICompilationManager>().Object,
                    new Mock<IPageActivator>().Object,
                    new Mock<IPageExecutor>().Object,
                    new Mock<ITraceFactory>().Object), "fileSystem");
            }

            [Fact]
            public void RequiresNonNullOrEmptyVirtualRoot()
            {
                ContractAssert.NotNullOrEmpty(s => new EdgeApplication(
                    new Mock<IFileSystem>().Object,
                    s,
                    new Mock<IRouter>().Object,
                    new Mock<ICompilationManager>().Object,
                    new Mock<IPageActivator>().Object,
                    new Mock<IPageExecutor>().Object,
                    new Mock<ITraceFactory>().Object), "virtualRoot");
            }

            [Fact]
            public void RequiresNonNullRouter()
            {
                ContractAssert.NotNull(() => new EdgeApplication(
                    new Mock<IFileSystem>().Object,
                    "Foo",
                    null,
                    new Mock<ICompilationManager>().Object,
                    new Mock<IPageActivator>().Object,
                    new Mock<IPageExecutor>().Object,
                    new Mock<ITraceFactory>().Object), "router");
            }

            [Fact]
            public void RequiresNonNullCompilationManager()
            {
                ContractAssert.NotNull(() => new EdgeApplication(
                    new Mock<IFileSystem>().Object,
                    "Foo",
                    new Mock<IRouter>().Object,
                    null,
                    new Mock<IPageActivator>().Object,
                    new Mock<IPageExecutor>().Object,
                    new Mock<ITraceFactory>().Object), "compiler");
            }

            [Fact]
            public void RequiresNonNullPageActivator()
            {
                ContractAssert.NotNull(() => new EdgeApplication(
                    new Mock<IFileSystem>().Object,
                    "Foo",
                    new Mock<IRouter>().Object,
                    new Mock<ICompilationManager>().Object,
                    null,
                    new Mock<IPageExecutor>().Object,
                    new Mock<ITraceFactory>().Object), "activator");
            }

            [Fact]
            public void RequiresNonNullPageExecutor()
            {
                ContractAssert.NotNull(() => new EdgeApplication(
                    new Mock<IFileSystem>().Object,
                    "Foo",
                    new Mock<IRouter>().Object,
                    new Mock<ICompilationManager>().Object,
                    new Mock<IPageActivator>().Object,
                    null,
                    new Mock<ITraceFactory>().Object), "executor");
            }

            [Fact]
            public void RequiresNonNullTracer()
            {
                ContractAssert.NotNull(() => new EdgeApplication(
                    new Mock<IFileSystem>().Object,
                    "Foo",
                    new Mock<IRouter>().Object,
                    new Mock<ICompilationManager>().Object,
                    new Mock<IPageActivator>().Object,
                    new Mock<IPageExecutor>().Object,
                    null), "tracer");
            }
        }
    }
}
