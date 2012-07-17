using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Edge.Execution;
using Gate;

namespace Edge
{
    public abstract class PageBase : IEdgePage
    {
        public Request Request { get; private set; }
        public Response Response { get; private set; }

        public async Task Run(Request req, Response resp)
        {
            Request = req;
            Response = resp;
            Execute();
        }

        public abstract void Execute();

        public virtual void Write(object text)
        {
            Response.Write(WebUtility.HtmlEncode(text.ToString()));
        }

        public virtual void WriteLiteral(object text)
        {
            Response.Write(text.ToString());
        }
    }
}
