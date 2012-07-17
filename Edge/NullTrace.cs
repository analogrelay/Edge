using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Edge
{
    public class NullTraceFactory : ITraceFactory
    {
        public static readonly NullTraceFactory Instance = new NullTraceFactory();

        private NullTraceFactory() { }

        public ITrace ForRequest(Gate.Request req)
        {
            return NullTrace.Instance;
        }

        public ITrace ForApplication()
        {
            return NullTrace.Instance;
        }
    }

    public class NullTrace : ITrace
    {
        public static readonly NullTrace Instance = new NullTrace();

        private NullTrace() { }

        public void WriteLine(string format, params object[] args)
        {
        }

        public IDisposable StartTrace()
        {
            return new DisposableAction(() => {});
        }
    }
}
