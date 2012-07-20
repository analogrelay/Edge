using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KillScreen.Interop;

namespace KillScreen
{
    public class ErrorDetail
    {
        public Uri UniqueId { get; private set; }
        public string Message { get; private set; }
        public FileLocation Location { get; private set; }
        public bool InGeneratedSource { get; private set; }

        public ErrorDetail(string message)
        {
            Message = message;
            Location = new FileLocation() { FileName = null };
        }

        public ErrorDetail(string message, FileLocation location)
        {
            Message = message;
            Location = location;
        }

        public ErrorDetail(string message, FileLocation location, bool inGeneratedSource)
        {
            Message = message;
            Location = location;
            InGeneratedSource = inGeneratedSource;
        }
    }
}
