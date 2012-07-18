using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Edge
{
    public interface IHttpException
    {
        public int StatusCode { get; }
        public string ReasonPhrase { get; }
    }
}
