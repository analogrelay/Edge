﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gate;

namespace Edge
{
    public interface ITraceFactory
    {
        ITrace ForRequest(Request req);
        ITrace ForApplication();
    }

    public interface ITrace
    {
        void WriteLine(string format, params object[] args);
        IDisposable StartTrace();
    }
}
