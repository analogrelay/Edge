using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Edge
{
    public interface IMultiErrorException
    {
        public IEnumerable<string> ErrorMessages { get; }
    }
}
