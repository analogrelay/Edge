﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edge.Execution;
using VibrantUtils;
using Xunit;

namespace Edge.Facts
{
    public class DefaultPageActivatorFacts
    {
        public class TheActivatePageMethod
        {
            [Fact]
            public void RequiresNonNullType()
            {
                ContractAssert.NotNull(() => new DefaultPageActivator().ActivatePage(null, NullTrace.Instance), "type");
            }

            [Fact]
            public void RequiresNonNullTracer()
            {
                ContractAssert.NotNull(() => new DefaultPageActivator().ActivatePage(typeof(object), null), "tracer");
            }

            [Fact]
            public void ReturnsSuccessfulResultIfTypeIsPubliclyConstructableEdgePage()
            {
                // Arrange
                var activator = new DefaultPageActivator();

                // Act
                var result = activator.ActivatePage(typeof(ConstructableEdgePage), NullTrace.Instance);

                // Assert
                Assert.True(result.Success);
                Assert.IsType<ConstructableEdgePage>(result.Page);
            }

            [Fact]
            public void ReturnsFailedResultIfTypeIsEdgePageButNotPubliclyConstructable()
            {
                // Arrange
                var activator = new DefaultPageActivator();

                // Act
                var result = activator.ActivatePage(typeof(NonConstructableEdgePage), NullTrace.Instance);

                // Assert
                Assert.False(result.Success);
                Assert.Null(result.Page);
            }

            [Fact]
            public void ReturnsFailedResultIfTypeIsEdgePageButHasNoPublicParameterlessCtor()
            {
                // Arrange
                var activator = new DefaultPageActivator();

                // Act
                var result = activator.ActivatePage(typeof(NoParameterlessConstructorEdgePage), NullTrace.Instance);

                // Assert
                Assert.False(result.Success);
                Assert.Null(result.Page);
            }

            [Fact]
            public void ReturnsFailedResultIfTypeIsConstructableButNotEdgePage()
            {
                // Arrange
                var activator = new DefaultPageActivator();

                // Act
                var result = activator.ActivatePage(typeof(object), NullTrace.Instance);

                // Assert
                Assert.False(result.Success);
                Assert.Null(result.Page);
            }
        }

        private class ConstructableEdgePage : IEdgePage
        {
            public Task Run(Gate.Request req, Gate.Response resp)
            {
                throw new NotImplementedException();
            }
        }

        private class NonConstructableEdgePage : IEdgePage
        {
            private NonConstructableEdgePage() { }
            public Task Run(Gate.Request req, Gate.Response resp)
            {
                throw new NotImplementedException();
            }
        }

        private class NoParameterlessConstructorEdgePage : IEdgePage
        {
            public NoParameterlessConstructorEdgePage(string foo) { }
            public Task Run(Gate.Request req, Gate.Response resp)
            {
                throw new NotImplementedException();
            }
        }
    }
}
