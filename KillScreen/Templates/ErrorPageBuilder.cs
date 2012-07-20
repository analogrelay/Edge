using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KillScreen.Templates
{
    public class ErrorPageBuilder
    {
        public ErrorSummary Summary { get; private set; }

        public ErrorPageBuilder(ErrorSummary error)
        {
            Summary = error;
        }

        public void Write(TextWriter target)
        {
            var sourceLines = (Summary.CompilationSource ?? String.Empty).Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            target.Write(@"<!DOCTYPE html>

<html lang=""en"" xmlns=""http://www.w3.org/1999/xhtml"">
<head>
    <meta charset=""utf-8"" />
    <title>An error has occurred</title>
    <style>
        body {
            background-color: #1ba1e2;
            color: #fff;
            font-family: 'Segoe UI', 'Helvetica Neue', 'Roboto', sans-serif;
            font-weight: lighter;
            padding: 3em;
            font-size: 12pt;
        }

        h1 {
            margin: 10px;
            padding: 0;
            font-weight: lighter;
            font-size: 10em;
            margin-top: -50px;
        }

        h2 {
            margin: 10px;
            padding: 0;
            font-weight: lighter;
            font-size: 3em;
        }

        h3 {
            margin: 10px;
            padding: 0;
            font-weight: lighter;
            font-size: 2em;
        }

        h4 {
            margin: 10px;
            padding: 0;
            font-weight: lighter;
            font-size: 1.5em;
        }

        h5 {
            margin: 10px;
            font-weight: lighter;
            font-size: 1em;
        }

        pre {
            margin: 10px;
            padding: 10px;
            background-color: white;
            color: black;
            border-top: solid #f09609 5px;
            overflow-x: auto;
        }

        ul#messages-list {
            margin: 10px;
            padding: 0;
        }

            ul#messages-list li {
                padding: 0px;
                margin-bottom: 10px;
                list-style-type: none;
                border-top: solid #f09609 5px;
                background-color: white;
                color: black;
                overflow-x: auto;
            }

                ul#messages-list li h5 {
                    margin: 0;
                    padding: 5px;
                    color: #e51400;
                    font-weight: lighter;
                    font-size: 1em;
                }

                ul#messages-list li p {
                    margin: 0;
                    padding: 5px;
                    font-weight: lighter;
                    font-size: 1em;
                }

                ul#messages-list li pre {
                    border: none;
                    margin: 0;
                    padding: 5px;
                    font-weight: lighter;
                    font-size: 1em;
                    color: black;
                    overflow-x: visible;
                }
    </style>
</head>
<body>
    <div id=""container"">
        <h1 id=""frowny"">:(</h1>
        <h2 id=""title"">An internal error has occurred</h2>
        <h3 id=""summary"">");
            target.Write(Summary.Summary);
            target.Write(@"</h3>");
            if(Summary.Errors != null && Summary.Errors.Any()) {
                target.Write(@"
        <h4 id=""messages-title"">");
                target.Write(Summary.ErrorListTitle);
                target.Write(@"</h4>
        <ul id=""messages-list"">");
                foreach(var error in Summary.Errors) {
                    target.Write(@"
            <li class=""message"">
                <h5 class=""message-title"">");
                    target.Write(error.Message);
                    target.Write(@"
                </h5>
                <div class=""message-body"">");
                    if(!String.IsNullOrEmpty(error.Location.FileName)) {
                        target.Write(@"
                    <pre class=""message-code"">");
                        WriteSourceCode(target, error, sourceLines);
                        target.Write(@"
                    </pre>");
                    }
                    target.Write(@"
                </div>");
                    if(error.Location.InGeneratedSource || !String.IsNullOrEmpty(error.Location.FileName)) {
                        target.Write(@"
                <p class=""message-location"">");
                        target.Write(error.Location.InGeneratedSource ? "[Generated Source Code]" : error.Location.FileName);
                        if (error.Location.LineNumber.HasValue)
                        {
                            target.Write(":");
                            target.Write(error.Location.LineNumber.Value);
                            if (error.Location.Column.HasValue)
                            {
                                target.Write(",");
                                target.Write(error.Location.Column.Value);
                            }
                        }
                        target.Write(@"</p>");
                    }
                    target.Write(@"
            </li>");
                }
                target.Write(@"
        </ul>");
            }
            if (Summary.Exception != null)
            {
                target.Write(@"
        <h4 id=""exception-detail"">Exception Details:</h4>
        <h5>");
                target.Write(Summary.Exception.GetType().FullName);
                target.Write(": ");
                target.Write(Summary.Exception.Message);
                target.Write(@"</h5>
        <pre id=""stack-trace"">");
                target.Write(Summary.Exception.StackTrace);
                target.Write(@"</pre>");
            }
            if(!String.IsNullOrEmpty(Summary.CompilationSource)) {
                target.Write(@"
        <h4 id=""compilation-source-header"">Full Compilation Source:</h4>
        <pre id=""compilation-source"">");
                target.Write(WebUtility.HtmlEncode(Summary.CompilationSource));
                target.Write(@"
        </pre>");
            }
            target.Write(@"
    </div>
</body>
</html>");
        }

        private void WriteSourceCode(TextWriter writer, ErrorDetail error, string[] compilationLines)
        {
            if (String.IsNullOrEmpty(error.Location.FileName) || !error.Location.LineNumber.HasValue)
            {
                writer.Write("&lt;Source Not Available&gt;");
                return;
            }
            
            string[] lines;
            if(error.Location.InGeneratedSource)
            {
                lines = compilationLines;
            }
            else if (File.Exists(error.Location.FileName))
            {
                // Super hacky!
                lines = File.ReadAllLines(error.Location.FileName);
            }
            else
            {
                writer.Write("&lt;Source Not Available&gt;");
                return;
            }

            string line = lines[error.Location.LineNumber.Value];
            writer.Write(String.Format("{0}:{1}", error.Location.LineNumber.Value, WebUtility.HtmlEncode(line)));
        }
    }
}
