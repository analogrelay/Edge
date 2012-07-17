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
        private static readonly MethodInfo TheStartMethod = typeof(EdgeApplication).GetMethod("Start", BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new[] { typeof(AppDelegate) }, new ParameterModifier[0]);

        private Stack<Delegate> _middleware = new Stack<Delegate>();

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
            _middleware.Push(middleware);
            return this;
        }

        public void VerifyStack(params Func<Delegate, bool>[] verifiers)
        {
            var middlewares = _middleware.Reverse().ToArray();
            for (int i = 0; i < _middleware.Count; i++)
            {
                Assert.True(verifiers[i](middlewares[i]));
            }
        }

        public static Func<Delegate, bool> IsEdgeApplication()
        {
            return del => VerifyEdgeApp(del.Target as EdgeApplication) && (del.Method == TheStartMethod);
        }

        private static bool VerifyEdgeApp(EdgeApplication app) 
        {
            return VerifyEdgeApp(app, String.Empty);
        }

        private static bool VerifyEdgeApp(EdgeApplication app, string virtualPath)
        {
            return VerifyEdgeApp(app, String.Empty, new PhysicalFileSystem(Environment.CurrentDirectory));
        }

        private static bool VerifyEdgeApp(EdgeApplication app, string virtualPath, IFileSystem expectedFs)
        {
            return app != null &&
                   String.Equals(app.VirtualRoot, virtualPath) &&
                   app.FileSystem.Equals(expectedFs);
        }
    }
}
