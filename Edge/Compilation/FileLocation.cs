using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Edge.Compilation
{
    public class FileLocation
    {
        public string FileName { get; private set; }
        public int? LineNumber { get; private set; }
        public int? Column { get; private set; }

        public FileLocation(string fileName) {
            FileName = fileName;
            LineNumber = null;
            Column = null;
        }

        public FileLocation(string fileName, int lineNumber, int column) : this(fileName)
        {
            LineNumber = lineNumber;
            Column = column;
        }

        public override string ToString()
        {
            return String.Concat(
                FileName,
                LineNumber == null ? String.Empty :
                    String.Format(":{0},{1}", LineNumber.Value, Column.Value));
        }
    }
}
