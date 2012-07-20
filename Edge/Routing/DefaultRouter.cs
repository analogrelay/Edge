using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edge.IO;
using Gate;
using VibrantUtils;

namespace Edge.Routing
{
    public class DefaultRouter : IRouter
    {
        private HashSet<string> _knownExtensions = new HashSet<string>(new string[] {
            ".cshtml"
        }, StringComparer.OrdinalIgnoreCase);
        
        private HashSet<string> _defaultDocumentNames = new HashSet<string>(new string[] {
            "Default",
            "Index"
        }, StringComparer.OrdinalIgnoreCase);

        public IFileSystem FileSystem { get; protected set; }

        public ISet<string> KnownExtensions
        {
            get { return _knownExtensions; }
        }

        public ISet<string> DefaultDocumentNames
        {
            get { return _defaultDocumentNames; }
        }

        protected DefaultRouter()
        {
        }

        public DefaultRouter(IFileSystem fileSystem)
        {
            Requires.NotNull(fileSystem, "fileSystem");

            FileSystem = fileSystem;
        }

        public Task<RouteResult> Route(Request request, ITrace tracer)
        {
            Requires.NotNull(request, "request");
            Requires.NotNull(tracer, "tracer");

            // This is so slooooow!
            string[] pathFragments = request.Path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            for (int end = pathFragments.Length - 1; end >= 0; end--)
            {
                Tuple<string, string> candidate = CreateCandidate(pathFragments, end);
                foreach (string extension in KnownExtensions)
                {
                    string physicalPath = candidate.Item1.Replace('/', Path.DirectorySeparatorChar);
                    IFile file = FileSystem.GetFile(physicalPath + extension);
                    if (file.Exists)
                    {
                        return Task.FromResult(RouteResult.Successful(file, candidate.Item2));
                    }
                    else
                    {
                        // Try "[name]/Default.cshtml"
                        foreach (string docNames in DefaultDocumentNames)
                        {
                            file = FileSystem.GetFile(Path.Combine(physicalPath, docNames + extension));
                            if (file.Exists)
                            {
                                return Task.FromResult(RouteResult.Successful(file, candidate.Item2));
                            }
                        }
                    }
                }
            }
            return Task.FromResult(RouteResult.Failed());
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
