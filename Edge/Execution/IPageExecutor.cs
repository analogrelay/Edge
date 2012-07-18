using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gate;

namespace Edge.Execution
{
    public interface IPageExecutor
    {
        Task<Response> Execute(IEdgePage page, Request req, ITrace tracer);
    }
}
