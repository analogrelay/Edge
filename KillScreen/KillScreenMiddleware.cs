using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Owin;

namespace KillScreen
{
    public class KillScreenMiddleware
    {
        public IList<IExceptionProcessor> Processors { get; private set; }

        public KillScreenMiddleware()
        {
            Processors = new List<IExceptionProcessor>();
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
            ErrorSummary details;
            foreach (IExceptionProcessor processor in Processors)
            {
                details = processor.Process(ex);
                if (details != null)
                {
                    break;
                }
            }

            // 
        }
    }
}
