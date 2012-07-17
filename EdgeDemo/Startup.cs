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
            app.UseEdge();
            
            // Enable console tracing
            //Trace.Listeners.Add(new ConsoleTraceListener());

            //app.Use<AppDelegate>(next => call =>
            //{
            //    Trace.WriteLine(String.Format("IN: {0} {1}", call.Environment["owin.RequestMethod"], call.Environment["owin.RequestPath"]));
            //    return next(call);
            //});
            //app.UseEdge();
            //app.Use<AppDelegate>(next => async call =>
            //{
            //    var result = await next(call);
            //    Trace.WriteLine(String.Format("OUT: {0}", result.Status));
            //    return result;
            //});
        }
    }
}