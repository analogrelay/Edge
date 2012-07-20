using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gate;
using KillScreen.Templates;
using Owin;

namespace KillScreen
{
    public class KillScreenMiddleware
    {
        public IList<IExceptionAnalyzer> Analyzers { get; private set; }

        public KillScreenMiddleware()
        {
            Analyzers = new List<IExceptionAnalyzer>() {
                new DefaultAnalyzer()
            };
        }

        public AppDelegate Start(AppDelegate next)
        {
            return async call =>
            {
                ResultParameters result;
                try
                {
                    result = await next(call);
                }
                catch (Exception ex)
                {
                    return HandleException(ex);
                }
                return result;
            };
        }

        private ResultParameters HandleException(Exception ex)
        {
            // Run the adapters
            ErrorSummary details = null;
            foreach (IExceptionAnalyzer analyzer in Analyzers)
            {
                details = analyzer.Analyze(ex);
                if (details != null)
                {
                    break;
                }
            }

            if (details == null)
            {
                details = GenerateNoProcessorsError(ex);
            }

            // Generate the HTML
            ErrorPageBuilder builder = new ErrorPageBuilder(details);

            Response resp = new Response(details.StatusCode);
            resp.Start();
            resp.ReasonPhrase = details.ReasonPhrase;
            resp.ContentType = "text/html";

            StringBuilder sb = new StringBuilder();
            using (StringWriter w = new StringWriter(sb))
            {
                builder.Write(w);
            }
            resp.Write(sb.ToString());
            resp.End();
            return resp.GetResultAsync().Result; //Good idea? Probably not...
        }

        private ErrorSummary GenerateNoProcessorsError(Exception ex)
        {
            return new ErrorSummary()
            {
                Exception = ex,
                Summary = ex.Message,
                StatusCode = 500,
                ReasonPhrase = "Server Error"
            };
        }
    }
}
