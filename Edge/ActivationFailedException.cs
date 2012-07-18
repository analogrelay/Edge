using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Edge
{
    public class ActivationFailedException : Exception
    {
        public Type AttemptedToActivate { get; private set; }

        public ActivationFailedException(Type attemptedToActivate)
            : base(FormatMessage(attemptedToActivate))
        {
            AttemptedToActivate = attemptedToActivate;
        }

        public ActivationFailedException(string message) : base(message) {}

        public ActivationFailedException(string message, Exception innerException) : base(message, innerException) {}

        public ActivationFailedException(SerializationInfo info, StreamingContext context) : base(info, context) {
            if (info.GetBoolean("AttemptedToActivate.HasValue"))
            {
                AttemptedToActivate = (Type)info.GetValue("AttemptedToActivate.Value", typeof(Type));
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("AttemptedToActivate.HasValue", AttemptedToActivate != null);
            if (AttemptedToActivate != null)
            {
                info.AddValue("AttemptedToActivate.Value", AttemptedToActivate);
            }
        }

        private static string FormatMessage(Type attemptedToActivate)
        {
            return String.Format(
                Strings.ActivationFailedException_DefaultMessage,
                attemptedToActivate.AssemblyQualifiedName);
        }
    }
}
