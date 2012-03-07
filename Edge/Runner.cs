using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gate;

namespace Edge
{
    public class Runner
    {
        public static void Run(Request req, Response resp, IFileSystem fs, string path)
        {
            PageCompileResults pageCompileResults = Compiler.Compile(fs, path + ".cshtml", req.TraceOutput);
            if (pageCompileResults.Success)
            {
                resp.Start("200 OK");
                pageCompileResults.Page.Run(resp);
            }
            else
            {
                resp.Start("500 Server Error");
                resp.Write(String.Join(System.Environment.NewLine, pageCompileResults.Errors));
                resp.Write("\r\n");
                resp.Write("Generated C# Code:\r\n");
                resp.Write(pageCompileResults.GeneratedCode);
                
            }
            resp.End();
        }
    }
}
