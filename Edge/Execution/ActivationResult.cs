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
        public Type ActivatedType { get; private set; }

        private ActivationResult(bool success, IEdgePage page, Type activatedType)
        {
            Success = success;
            Page = page;
            ActivatedType = activatedType;
        }

        public static ActivationResult Failed(Type activatedType)
        {
            return new ActivationResult(false, null, activatedType);
        }

        public static ActivationResult Successful(IEdgePage page, Type activatedType)
        {
            return new ActivationResult(true, page, activatedType);
        }
    }
}
