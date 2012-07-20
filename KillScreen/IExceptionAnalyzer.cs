using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KillScreen
{
    public interface IExceptionAnalyzer
    {
        ErrorSummary Analyze(Exception ex);
    }
}
