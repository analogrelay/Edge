using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edge.Execution;
using Gate;
using Moq;
using Owin;
using VibrantUtils;
using Xunit;

namespace Edge.Facts
{
    public class DefaultPageExecutorFacts
    {
        public class TheExecuteMethod
        {
            [Fact]
            public void RequiresNonNullPage()
            {
                ContractAssert.NotNull(() => new DefaultPageExecutor().Execute(
                    null,
                    new Request(new CallParameters()),
                    NullTrace.Instance), "page");
            }

            [Fact]
            public void RequiresNonNullRequest()
            {
                ContractAssert.NotNull(() => new DefaultPageExecutor().Execute(
                    new Mock<IEdgePage>().Object,
                    null,
                    NullTrace.Instance), "request");
            }

            [Fact]
            public void RequiresNonNullTracer()
            {
                ContractAssert.NotNull(() => new DefaultPageExecutor().Execute(
                    new Mock<IEdgePage>().Object,
                    new Request(new CallParameters()),
                    null), "tracer");
            }

            [Fact]
            public async Task Returns200ResponseAndExecutesPage()
            {
                // Arrange
                var page = new Mock<IEdgePage>();
                var executor = new DefaultPageExecutor();
                var request = TestData.CreateRequest(path: "/Bar");

                page.Setup(p => p.Run(It.IsAny<Request>(), It.IsAny<Response>()))
                    .Returns((Request req, Response res) =>
                    {
                        res.ReasonPhrase = "All good bro";
                        return Task.FromResult(new object());
                    });

                // Act
                var response = await executor.Execute(page.Object, request, NullTrace.Instance);

                // Assert
                page.Verify(p => p.Run(request, response));
                Assert.Equal(200, response.StatusCode);
                Assert.Equal("All good bro", response.ReasonPhrase);
            }
        }
    }
}
