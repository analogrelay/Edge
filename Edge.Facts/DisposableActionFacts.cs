using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VibrantUtils;
using Xunit;

namespace Edge.Facts
{
    public class DisposableActionFacts
    {
        public class TheConstructor
        {
            [Fact]
            public void RequiresNonNullAction()
            {
                ContractAssert.NotNull(() => new DisposableAction(null), "act");
            }
        }

        public class TheDisposeMethod
        {
            [Fact]
            public void InvokesTheAction()
            {
                bool invoked = false;
                new DisposableAction(() => invoked = true).Dispose();
                Assert.True(invoked);
            }
        }
    }
}
