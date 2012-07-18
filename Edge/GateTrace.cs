using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Gate;

namespace Edge
{
    public class GateTraceFactory : ITraceFactory
    {
        private long _nextId = 0;

        public ITrace ForRequest(Request req)
        {
            // Just loop around if we reach the end of the request id space.
            Interlocked.CompareExchange(ref _nextId, 0, Int64.MaxValue);

            return new GateTrace(req, Interlocked.Increment(ref _nextId));
        }

        public ITrace ForApplication()
        {
            return GateTrace.Global;
        }

        internal void SetCurrentId(long id)
        {
            _nextId = id;
        }
    }

    public class GateTrace : ITrace
    {
        public long RequestId { get; private set; }
        public Request Request { get; private set; }

        public static readonly ITrace Global = new GateTrace();

        private GateTrace()
        {
            Request = null;
            RequestId = -1;
        }

        public GateTrace(Request request, long id)
        {
            RequestId = id;
            Request = request;
        }

        public IDisposable StartTrace()
        {
            TraceMessage("Trace Started");
            return new DisposableAction(() => TraceMessage("Trace Complete"));
        }

        public void WriteLine(string format, params object[] args)
        {
            TraceMessage(format, args);
        }

        private void TraceMessage(string format, params object[] args) {
            string message = String.Format(format, args);
            if (Request != null)
            {
                Request.TraceOutput.WriteLine("[{2}][EDGE #{0}]: {1}", RequestId, message, DateTime.Now.ToString("HH:mm:ss.fff"));
            }
            else
            {
                Trace.WriteLine(String.Format("[{1}][EDGE Global]: {0}", message, DateTime.Now.ToString("HH:mm:ss.fff")));
            }
        }
    }
}
