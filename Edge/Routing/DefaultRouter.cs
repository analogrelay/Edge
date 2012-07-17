using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edge.IO;
using Gate;

namespace Edge.Routing
{
    public class DefaultRouter : IRouter
    {
        private static readonly HashSet<string> KnownExtensions = new HashSet<string>(new string[] {
            ".cshtml"
        });

        public IFileSystem FileSystem { get; private set; }

        public DefaultRouter(IFileSystem fileSystem)
        {
            FileSystem = fileSystem;
        }

        public async Task<RouteResult> Route(Request req)
        {
            // Start by just adding ".cshtml" to see if we can find a matching file
            string[] pathFragments = req.Path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            for (int end = pathFragments.Length - 1; end >= 0; end--)
            {
                Tuple<string, string> candidate = CreateCandidate(pathFragments, end);
                foreach (string extension in KnownExtensions)
                {
                    IFile file = FileSystem.GetFile(candidate.Item1 + extension);
                    if (file.Exists)
                    {
                        return RouteResult.Successful(file, candidate.Item2);
                    }
                }
            }
            return RouteResult.Failed();
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
