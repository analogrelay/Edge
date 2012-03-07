using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Razor;

namespace Edge
{
    public class PageCompileResults
    {
        public Page Page { get; private set; }
        public IList<string> Errors { get; private set; }
        public bool Success { get; private set; }
        public string GeneratedCode { get; private set; }

        public PageCompileResults(Page page) : this(true, String.Empty, page, new List<string>()) { }
        public PageCompileResults(string code, IEnumerable<string> errors) : this(false, code, null, errors) { }

        private PageCompileResults(bool success, string code, Page page, IEnumerable<string> errors)
        {
            GeneratedCode = code;
            Success = success;
            Page = page;
            Errors = errors.ToList();
        }
    }
}
