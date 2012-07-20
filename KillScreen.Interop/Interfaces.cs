using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

[assembly: ImportedFromTypeLib("KillScreen.tlb")]

namespace KillScreen.Interop
{
    [ComImport]
    [Guid("513FD052-1313-41FB-9E51-164E7387D84E")]
    public interface IHttpException
    {
        int StatusCode { get; }
        string ReasonPhrase { get; }
    }

    [ComImport]
    [Guid("1D42BE62-1DD2-4AA5-A17B-67A607D2E188")]
    public interface IMultiMessageException
    {
        string MessageListTitle { get; }
        IEnumerable<IErrorMessage> Messages { get; }
    }

    [ComImport]
    [Guid("950154BC-2E29-4AC1-9DDC-84608E0E49A5")]
    public interface IErrorMessage
    {
        FileLocation Location { get; }
        string Message { get; }
    }

    [ComImport]
    [Guid("11060B6A-0E69-41F9-A842-28DA86D82128")]
    public interface IProvidesCompilationSource
    {
        string CompilationSource { get; }
    }

    public struct FileLocation
    {
        public string FileName;
        public bool InGeneratedSource;
        public int? LineNumber;
        public int? Column;
    }
}
