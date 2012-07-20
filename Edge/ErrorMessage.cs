using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Edge.Compilation;
using KillScreen.Interop;
using KSFileLocation = KillScreen.Interop.FileLocation;

namespace Edge
{
    public class ErrorMessage : IErrorMessage
    {
        public KSFileLocation Location { get; set; }
        public string Message { get; set; }

        public ErrorMessage(string message)
        {
            Message = message;
            Location = new KSFileLocation()
            {
                FileName = null
            };
        }

        public ErrorMessage(string message, KSFileLocation location)
        {
            Message = message;
            Location = location;
        }

        public ErrorMessage(CompilationMessage cm)
        {
            Message = cm.Message;
            Location = new KSFileLocation()
            {
                FileName = cm.Location.FileName,
                LineNumber = cm.Location.LineNumber,
                Column = cm.Location.Column
            };
        }
    }
}
