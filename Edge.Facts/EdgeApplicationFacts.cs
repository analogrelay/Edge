using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edge.Compilation;
using Edge.Execution;
using Edge.IO;
using Edge.Routing;
using Gate;
using Moq;
using Owin;
using VibrantUtils;
using Xunit;
using Edge;

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

        public class TheStartMethod
        {
            [Fact]
            public async Task DelegatesIfIncomingIsNotUnderVirtualRoot()
            {
                // Arrange
                DelegationTracker delegation = new DelegationTracker();

                var app = CreateEdgeApp("/Foo");
                var appDel = app.Start(delegation.Next);

                // Act
                await appDel(TestData.CreateCallParams(path: "/Bar"));

                // Assert
                Assert.True(delegation.NextWasCalled);
            }

            [Fact]
            public async Task DelegatesIfIncomingIsNotMatchedByARoute()
            {
                // Arrange
                DelegationTracker delegation = new DelegationTracker();

                var app = CreateEdgeApp();
                var appDel = app.Start(delegation.Next);

                // Act
                await appDel(TestData.CreateCallParams(path: "/Bar"));

                // Assert
                Assert.True(delegation.NextWasCalled);
            }

            [Fact]
            public async Task ThrowsCompilationExceptionIfCompilationFails()
            {
                // Arrange
                var app = CreateEdgeApp();
                var appDel = app.Start();

                var testFile = app.TestFileSystem.AddTestFile("Bar.cshtml", "Flarg");

                var expected = new List<CompilationMessage>() {
                    new CompilationMessage(MessageLevel.Error, "Yar!"),
                    new CompilationMessage(MessageLevel.Warning, "Gar!", new FileLocation("Blar.cshtml")),
                    new CompilationMessage(MessageLevel.Info, "Far!", new FileLocation("War.cshtml", 10, 12))
                };

                app.MockCompilationManager
                   .Setup(c => c.Compile(testFile, It.IsAny<ITrace>()))
                   .Returns(Task.FromResult(CompilationResult.Failed(expected)));

                // Act
                var ex = await AssertEx.Throws<CompilationFailedException>(async () => await appDel(TestData.CreateCallParams(path: "/Bar")));

                // Assert
                Assert.Equal(
                    String.Format(Strings.CompilationFailedException_MessageWithErrorCounts, 1, 1),
                    ex.Message);
                Assert.Equal(
                    expected,
                    ex.Messages);
            }

            [Fact]
            public async Task ThrowsActivationExceptionIfActivationFails()
            {
                // Arrange
                var app = CreateEdgeApp();
                var appDel = app.Start();

                var testFile = app.TestFileSystem.AddTestFile("Bar.cshtml", "Flarg");

                Type compiled = typeof(EdgeApplicationFacts);
                app.MockCompilationManager
                   .Setup(c => c.Compile(testFile, It.IsAny<ITrace>()))
                   .Returns(Task.FromResult(CompilationResult.Successful(compiled, Enumerable.Empty<CompilationMessage>())));
                app.MockActivator
                   .Setup(a => a.ActivatePage(compiled, It.IsAny<ITrace>()))
                   .Returns(ActivationResult.Failed());

                // Act
                var ex = await AssertEx.Throws<ActivationFailedException>(async () => await appDel(TestData.CreateCallParams(path: "/Bar")));

                // Assert
                Assert.Equal(
                    String.Format(Strings.ActivationFailedException_DefaultMessage, compiled.AssemblyQualifiedName),
                    ex.Message);
                Assert.Equal(
                    compiled,
                    ex.AttemptedToActivate);
            }

            [Fact]
            public async Task ReturnsResultOfCallingExecutorIfAllPhasesSucceed()
            {
                // Arrange
                var app = CreateEdgeApp();
                var appDel = app.Start();

                var testFile = app.TestFileSystem.AddTestFile("Bar.cshtml", "Flarg");

                Type compiled = typeof(EdgeApplicationFacts);
                Mock<IEdgePage> page = new Mock<IEdgePage>();
                Response resp = new Response(418)
                {
                    ReasonPhrase = "I'm a teapot"
                };
                resp.Start();

                app.MockCompilationManager
                   .Setup(c => c.Compile(testFile, It.IsAny<ITrace>()))
                   .Returns(Task.FromResult(CompilationResult.Successful(compiled, Enumerable.Empty<CompilationMessage>())));
                app.MockActivator
                   .Setup(a => a.ActivatePage(compiled, It.IsAny<ITrace>()))
                   .Returns(ActivationResult.Successful(page.Object));
                app.MockExecutor
                   .Setup(e => e.Execute(page.Object, It.IsAny<Request>(), It.IsAny<ITrace>()))
                   .Returns(Task.FromResult(resp));

                // Act
                var result = await appDel(TestData.CreateCallParams(path: "/Bar"));

                // Assert
                Assert.Equal(418, result.Status);
                Assert.Equal("I'm a teapot", result.Properties["owin.ReasonPhrase"]);
            }
        }

        private static TestableEdgeApplication CreateEdgeApp(string virtualRoot = "/")
        {
            return new TestableEdgeApplication(virtualRoot);
        }

        private class TestableEdgeApplication : EdgeApplication
        {
            public TestFileSystem TestFileSystem { get; private set; }
            public Mock<ICompilationManager> MockCompilationManager { get; private set; }
            public Mock<IPageActivator> MockActivator { get; private set; }
            public Mock<IPageExecutor> MockExecutor { get; private set; }
            
            public TestableEdgeApplication(string virtualRoot) : base() {
                VirtualRoot = virtualRoot;
                FileSystem = TestFileSystem = new TestFileSystem(@"C:\Test");
                Router = new DefaultRouter(TestFileSystem);
                CompilationManager = (MockCompilationManager = new Mock<ICompilationManager>()).Object;
                Activator = (MockActivator = new Mock<IPageActivator>()).Object;
                Executor = (MockExecutor = new Mock<IPageExecutor>()).Object;
                Tracer = NullTraceFactory.Instance;
            }
        }
    }
}
