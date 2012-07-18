using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Edge.Execution
{
    public class ActivationResult
    {
        public bool Success { get; private set; }
        public IEdgePage Page { get; private set; }

        private ActivationResult(bool success, IEdgePage page)
        {
            Success = success;
            Page = page;
        }

        public static ActivationResult Failed()
        {
            return new ActivationResult(false, null);
        }

        public static ActivationResult Successful(IEdgePage page)
        {
            return new ActivationResult(true, page);
        }
    }
}
