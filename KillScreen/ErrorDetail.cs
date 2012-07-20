using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KillScreen
{
    public class ErrorDetail
    {
        public Guid UniqueId { get; private set; }
        public string UserMessage { get; private set; }
        public string DetailMessage { get; private set; }
        public IEnumerable<ErrorSolution> KnownSolutions { get; private set; }        
    }
}
