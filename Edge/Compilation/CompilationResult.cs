using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Edge.Execution;

namespace Edge.Compilation
{
    public class CompilationResult
    {
        private Type _type;

        public bool Success { get; private set; }
        public IList<CompilationMessage> Messages { get; private set; }

        public Type GetCompiledType()
        {
            if (_type == null)
            {
                throw new InvalidOperationException("Compilation Failed. There is no compiled Type");
            }
            return _type;
        }

        private CompilationResult(bool success, IList<CompilationMessage> messages, Type typ)
        {
            Success = success;
            Messages = messages;
            _type = typ;
        }

        public static CompilationResult Failed(IEnumerable<CompilationMessage> messages)
        {
            return new CompilationResult(false, messages.ToList(), null);
        }

        public static CompilationResult Successful(Type typ, IEnumerable<CompilationMessage> messages)
        {
            return new CompilationResult(true, messages.ToList(), typ);
        }
    }
}
