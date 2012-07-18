using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Edge.Compilation
{
    [Serializable]
    public class CompilationMessage
    {
        public MessageLevel Level { get; private set; }
        public FileLocation Location { get; private set; }
        public string Message { get; private set; }

        public CompilationMessage(MessageLevel level, string message) : this(level, message, null)
        {
        }

        public CompilationMessage(MessageLevel level, string message, FileLocation location)
        {
            Level = level;
            Message = message;
            Location = location;
        }

        public override string ToString()
        {
            return String.Format("[{0}]{1} - {2}",
                Level,
                Location == null ? String.Empty : (" (" + Location.ToString() + ")"),
                Message);
        }
    }
}
