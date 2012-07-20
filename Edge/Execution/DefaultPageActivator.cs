using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VibrantUtils;

namespace Edge.Execution
{
    public class DefaultPageActivator : IPageActivator
    {
        public ActivationResult ActivatePage(Type type, ITrace tracer)
        {
            Requires.NotNull(type, "type");
            Requires.NotNull(tracer, "tracer");

            IEdgePage page = null;
            try
            {
                page = Activator.CreateInstance(type) as IEdgePage;
            }
            catch (MissingMethodException)
            {
                return ActivationResult.Failed();
            }

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
