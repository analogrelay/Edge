using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gate.Builder;
using Xunit;

namespace Edge.Facts
{
    public class AppBuilderExtensionsFacts
    {
        public class TheUseEdgeMethod
        {
            public class WithNoArguments
            {
                [Fact]
                public void RequiresNonNullBuilder()
                {
                    var ane = Assert.Throws<ArgumentNullException>(() => AppBuilderExtensions.UseEdge(null));
                    Assert.Equal("builder", ane.ParamName);
                }

                [Fact]
                public void ConfiguresEdgeAppForCurrentDirectoryAtRootVirtualPath()
                {
                    // Arrange
                    TestAppBuilder builder = new TestAppBuilder();

                    // Act
                    builder.UseEdge();

                    // Assert
                    builder.VerifyStack(
                        TestAppBuilder.IsEdgeApplication());
                }
            }
        }
    }
}
