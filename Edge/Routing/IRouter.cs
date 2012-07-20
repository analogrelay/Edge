using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gate;

namespace Edge.Routing
{
    public interface IRouter
    {
        Task<RouteResult> Route(Request request, ITrace tracer);
    }
}
