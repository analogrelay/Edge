using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Edge.IO;

namespace Edge.Routing
{
    public class RouteResult
    {
        public bool Success { get; private set; }
        public IFile File { get; private set; }
        public string PathInfo { get; private set; }

        private RouteResult(bool success, IFile file, string pathInfo)
        {
            Success = success;
            File = file;
            PathInfo = pathInfo;
        }

        public static RouteResult Failed()
        {
            return new RouteResult(false, null, null);
        }

        public static RouteResult Successful(IFile file, string pathInfo)
        {
            return new RouteResult(true, file, pathInfo);
        }
    }
}
