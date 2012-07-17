using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Edge.Execution
{
    public interface IPageActivator
    {
        ActivationResult ActivatePage(Type type);
    }
}
