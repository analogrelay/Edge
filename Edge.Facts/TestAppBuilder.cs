using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Edge.IO;
using Owin;
using Xunit;

namespace Edge.Facts
{
    public class TestAppBuilder : IAppBuilder
    {
        public Stack<Delegate> MiddlewareStack { get; private set; }

        public TestAppBuilder()
        {
            MiddlewareStack = new Stack<Delegate>();
        }

        public IAppBuilder AddAdapters<TApp1, TApp2>(Func<TApp1, TApp2> adapter1, Func<TApp2, TApp1> adapter2)
        {
            throw new NotImplementedException();
        }

        public TApp Build<TApp>(Action<IAppBuilder> pipeline)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, object> Properties
        {
            get { throw new NotImplementedException(); }
        }

        public IAppBuilder Use<TApp>(Func<TApp, TApp> middleware)
        {
            MiddlewareStack.Push(middleware);
            return this;
        }
    }
}
