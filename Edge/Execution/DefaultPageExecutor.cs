using System;
using System.Threading.Tasks;
using Gate;

namespace Edge.Execution
{
    public class DefaultPageExecutor : IPageExecutor
    {
        public async Task<Response> Execute(IEdgePage page, Request req)
        {
            Response resp = new Response(200);
            resp.Start();
            await page.Run(req, resp);
            resp.End();
            return resp;
        }
    }
}
