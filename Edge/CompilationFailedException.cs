using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Edge.Compilation;

namespace Edge
{
    [Serializable]
    public class CompilationFailedException : Exception
    {
        public IList<CompilationMessage> Messages { get; private set; }

        public CompilationFailedException()
            : base(Strings.CompilationFailedException_DefaultMessage)
        {
            Messages = new List<CompilationMessage>();
        }

        public CompilationFailedException(IEnumerable<CompilationMessage> messages)
            : base(FormatMessage(messages))
        {
            Messages = new List<CompilationMessage>();
        }

        public CompilationFailedException(string message) : base(message) {
            Messages = new List<CompilationMessage>();
        }

        public CompilationFailedException(string message, Exception innerException) : base(message, innerException) {
            Messages = new List<CompilationMessage>();
        }

        public CompilationFailedException(SerializationInfo info, StreamingContext context) : base(info, context) {
            CompilationMessage[] messages = new CompilationMessage[info.GetInt32("Messages.Count")];
            for (int i = 0; i < messages.Length; i++)
            {
                messages[i] = (CompilationMessage)info.GetValue("Messages[" + i + "]", typeof(CompilationMessage));
            }
            Messages = messages.ToList();
        }

        private static string FormatMessage(IEnumerable<CompilationMessage> messages)
        {
            var counts = messages.Aggregate(Tuple.Create(0, 0), (last, current) =>
                Tuple.Create(
                    last.Item1 + (current.Level == MessageLevel.Error ? 1 : 0),
                    last.Item2 + (current.Level == MessageLevel.Warning ? 1 : 0)));

            return String.Format(Strings.CompilationFailedException_MessageWithErrorCounts, counts.Item1, counts.Item2);
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Messages.Count", Messages.Count);
            for (int i = 0; i < Messages.Count; i++)
            {
                info.AddValue("Messages[" + i + "]", Messages[i]);
            }
        }
    }
}
