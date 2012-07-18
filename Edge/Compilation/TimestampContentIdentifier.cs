using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edge.IO;

namespace Edge.Compilation
{
    public class TimestampContentIdentifier : IContentIdentifier
    {
        public string GenerateContentId(IFile file)
        {
            return String.Format("{0}@{1}", file.FullPath, file.LastModifiedTime.Ticks);
        }
    }
}
