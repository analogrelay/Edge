using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Edge.Execution
{
    public class DefaultPageActivator : IPageActivator
    {
        public ActivationResult ActivatePage(Type type, ITrace tracer)
        {
            IEdgePage page = Activator.CreateInstance(type) as IEdgePage;
            if (page == null)
            {
                return ActivationResult.Failed();
            }
            else
            {
                return ActivationResult.Successful(page);
            }
        }
    }
}
