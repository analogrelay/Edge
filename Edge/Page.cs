using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gate;

namespace Edge
{
    public abstract class Page
    {
        public Response Response { get; private set; }

        public void Run(Response resp)
        {
            Response = resp;
            Execute();
        }

        public abstract void Execute();

        public virtual void Write(object value)
        {
            WriteLiteral("!" + value.ToString() + "!");
        }

        public virtual void WriteLiteral(object value)
        {
            Response.Write(value.ToString());
        }
    }
}
