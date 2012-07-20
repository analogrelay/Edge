using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KillScreen
{
    public class ErrorSummary
    {
        public string Summary { get; set; }
        public string ErrorListTitle { get; set; }
        public Exception Exception { get; set; }
        public IEnumerable<ErrorDetail> Errors { get; set; }
    }
}
