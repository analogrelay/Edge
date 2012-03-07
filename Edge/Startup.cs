using Owin;
using Gate;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using Gate.Middleware;
using System.Diagnostics;

namespace Edge
{
    public static class Startup
    {
        public static void Configuration(IAppBuilder app)
        {
            string root = System.Environment.CurrentDirectory;
            app.UseShowExceptions();
            app.RunDirect((req, resp) =>
            {
                req.TraceOutput.WriteLine("{0} {1}", req.Method, req.Path);

                IFileSystem fs = new PhysicalFileSystem(root);

                // Map the incoming url to a file and params pair
                Tuple<string, string> pair = Route(fs, req.Path);
                if (pair == null)
                {
                    req.TraceOutput.WriteLine("\tNo matching route!");
                }
                else
                {
                    req.TraceOutput.WriteLine("\tRouted to: " + pair.Item1 + ".cshtml");
                    req.TraceOutput.WriteLine("\tWith data: " + pair.Item2);

                    // Run the page
                    Runner.Run(req, resp, fs, pair.Item1);
                }
                resp.End();
            });
        }

        private static Tuple<string, string> Route(IFileSystem fs, string path)
        {
            // Start by just adding ".cshtml" to see if we can find a matching file
            string[] pathFragments = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            for (int end = pathFragments.Length - 1; end >= 0; end--)
            {
                Tuple<string, string> candidate = CreateCandidate(pathFragments, end);
                if (fs.Exists(candidate.Item1 + ".cshtml"))
                {
                    return candidate;
                }
            }
            return null;
        }

        private static Tuple<string, string> CreateCandidate(string[] pathFragments, int end)
        {
            // TODO: Shortcuts, precalcuate string lengths, etc.

            StringBuilder pathBuilder = new StringBuilder();
            StringBuilder dataBuilder = new StringBuilder();
            for (int i = 0; i < pathFragments.Length; i++)
            {
                if (i > 0 && i < end + 1)
                {
                    pathBuilder.Append("/");
                }
                else if (i > end + 1)
                {
                    dataBuilder.Append("/");
                }

                if (i <= end)
                {
                    pathBuilder.Append(pathFragments[i]);
                }
                else
                {
                    dataBuilder.Append(pathFragments[i]);
                }
            }
            return Tuple.Create(pathBuilder.ToString(), dataBuilder.ToString());
        }
    }
}
