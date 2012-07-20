using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KillScreen.Interop;

namespace KillScreen
{
    public class DefaultAnalyzer : IExceptionAnalyzer
    {
        public ErrorSummary Analyze(Exception ex)
        {
            IHttpException http = ex as IHttpException;
            IMultiMessageException mm = ex as IMultiMessageException;
            IProvidesCompilationSource compsrc = ex as IProvidesCompilationSource;
            return new ErrorSummary()
            {
                StatusCode = http != null ? http.StatusCode : 500,
                ReasonPhrase = http != null ? http.ReasonPhrase : "Server Error",
                Summary = ex.Message,
                Exception = ex,
                ErrorListTitle = mm != null ? mm.MessageListTitle : null,
                Errors = mm != null ? mm.Messages.Select(ConvertMessage) : null,
                CompilationSource = compsrc != null ? compsrc.CompilationSource : null
            };
        }

        private ErrorDetail ConvertMessage(IErrorMessage arg)
        {
            return new ErrorDetail(arg.Message, arg.Location);
        }
    }
}
