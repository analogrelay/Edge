using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gate;

namespace Edge.Execution
{
    public interface IEdgePage
    {
        Task Run(Request req, Response resp);
    }
}
