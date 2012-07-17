using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Owin;

namespace Edge.Facts
{
    public class DelegationTracker
    {
        public bool NextWasCalled { get; private set; }
        public CallParameters NextCallParams { get; private set; }
        public AppDelegate Next { get; private set; }

        public DelegationTracker()
        {
            Next = cp =>
            {
                NextCallParams = cp;
                NextWasCalled = true;
                return Task.FromResult(new ResultParameters());
            };
        }
    }
}
