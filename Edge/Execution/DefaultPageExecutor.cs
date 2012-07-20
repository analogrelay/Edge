using System;
using System.Threading.Tasks;
using Gate;
using VibrantUtils;

namespace Edge.Execution
{
    public class DefaultPageExecutor : IPageExecutor
    {
        public Task<Response> Execute(IEdgePage page, Request request, ITrace tracer)
        {
            Requires.NotNull(page, "page");
            Requires.NotNull(request, "request");
            Requires.NotNull(tracer, "tracer");

            return ExecuteCore(page, request, tracer);
        }

        private static async Task<Response> ExecuteCore(IEdgePage page, Request request, ITrace tracer)
        {
            Response resp = new Response(200);
            resp.Start();
            await page.Run(request, resp);
            resp.End();
            return resp;
        }
    }
}
