using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KillScreen
{
    public interface IExceptionProcessor
    {
        ErrorSummary Process(Exception ex);
    }
}
