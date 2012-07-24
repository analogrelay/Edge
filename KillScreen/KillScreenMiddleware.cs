using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            return new ResultParameters
            {
                Status = details.StatusCode,
                Properties = new Dictionary<string, object>() 
                {
                    {"owin.ReasonPhrase", details.ReasonPhrase},
                },
                Headers = new Dictionary<string, string[]>() 
                {
                    {"Content-Type", new[]{"text/html"}}
                },
                Body = (output, cancel) =>
                {
                    using (var writer = new StreamWriter(output))
                    {
                        // Generate the HTML
                        var builder = new ErrorPageBuilder(details);
                        builder.Write(writer);
                    }
                    return TaskHelpers.Completed();
                }
            };
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
