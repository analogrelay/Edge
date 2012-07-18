using System;
using System.Diagnostics;
using Edge;
using Gate.Middleware;
using Owin;

namespace EdgeDemo
{
    public class Startup
    {
        public static void Configuration(IAppBuilder app)
        {
            app.UseErrorPage();
            app.UseEdge();
        }
    }
}