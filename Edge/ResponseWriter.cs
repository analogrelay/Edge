using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Gate;

namespace Edge
{
    public class ResponseWriter : TextWriter
    {
        public Response Response { get; private set; }

        public ResponseWriter(Response response)
        {
            Response = response;
        }

        public override Encoding Encoding
        {
            get { return Response.Encoding; }
        }

        public override void Write(char c)
        {
            Response.Write(c.ToString());
        }
    }
}
