using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Razor;
using Edge.Execution;
using Edge.IO;
using Microsoft.CSharp;
using Roslyn.Compilers;
using Roslyn.Compilers.CSharp;
using CSCompilation = Roslyn.Compilers.CSharp.Compilation;

namespace Edge.Compilation
{
    public class RazorCompiler : ICompiler
    {
        private static Regex InvalidClassNameChars = new Regex("[^A-Za-z0-9_]");
        private static Dictionary<DiagnosticSeverity, MessageLevel> SeverityMap = new Dictionary<DiagnosticSeverity, MessageLevel>() {
            { DiagnosticSeverity.Error, MessageLevel.Error },
            { DiagnosticSeverity.Info, MessageLevel.Info },
            { DiagnosticSeverity.Warning, MessageLevel.Warning }
        };

        public bool CanCompile(IFile file)
        {
            return String.Equals(file.Extension, ".cshtml");
        }

        public Task<CompilationResult> Compile(IFile file)
        {
            string className = MakeClassName(file.Name);
            RazorTemplateEngine engine = new RazorTemplateEngine(new RazorEngineHost(new CSharpRazorCodeLanguage())
            {
                DefaultBaseClass = "Edge.PageBase"
            });

            GeneratorResults results;
            using (TextReader rdr = file.OpenRead())
            {
                results = engine.GenerateCode(rdr, className, "EdgeCompiled", file.FullPath);
            }

            List<CompilationMessage> messages = new List<CompilationMessage>();
            if (!results.Success)
            {
                foreach (var error in results.ParserErrors)
                {
                    messages.Add(new CompilationMessage(
                        MessageLevel.Error,
                        error.Message,
                        new FileLocation(file.FullPath, error.Location.LineIndex, error.Location.CharacterIndex)));
                }
            }

            // Regardless of success or failure, we're going to try and compile
            return Task.FromResult(CompileCSharp("EdgeCompiled." + className, file, results.Success, messages, results.GeneratedCode));
        }

        private CompilationResult CompileCSharp(string fullClassName, IFile file, bool success, List<CompilationMessage> messages, CodeCompileUnit codeCompileUnit)
        {
            // Generate code text
            StringBuilder code = new StringBuilder();
            CSharpCodeProvider provider = new CSharpCodeProvider();
            using (StringWriter writer = new StringWriter(code))
            {
                provider.GenerateCodeFromCompileUnit(codeCompileUnit, writer, new CodeGeneratorOptions());
            }

            // Parse
            SyntaxTree tree = SyntaxTree.ParseCompilationUnit(code.ToString(), "__Generated.cs");

            // Create a compilation
            CSCompilation comp = CSCompilation.Create(
                "Compiled",
                CompilationOptions.Default
                                  .WithOutputKind(OutputKind.DynamicallyLinkedLibrary), 
                syntaxTrees: new [] { tree }, 
                references: new [] {
                    new AssemblyFileReference(typeof(object).Assembly.Location),
                    new AssemblyFileReference(typeof(PageBase).Assembly.Location),
                    new AssemblyFileReference(typeof(Gate.Request).Assembly.Location)
                });

            // Emit to a collectable assembly
            AssemblyBuilder asm = AssemblyBuilder.DefineDynamicAssembly(
                new AssemblyName("Edge_" + Guid.NewGuid().ToString("N")), AssemblyBuilderAccess.RunAndCollect);
            ModuleBuilder mod = asm.DefineDynamicModule("EdgeCompilation");
            ReflectionEmitResult result = comp.Emit(mod);

            // Extract the type
            Type typ = null;
            if(result.Success) {
                typ = asm.GetType(fullClassName);
            }
            else {
                foreach(var diagnostic in result.Diagnostics) {
                    var span = diagnostic.Location.GetLineSpan(true);
                    var linePosition = span.StartLinePosition;
                    messages.Add(new CompilationMessage(
                        SeverityMap[diagnostic.Info.Severity], 
                        diagnostic.Info.GetMessage(), 
                        new FileLocation(
                            span.Path,
                            linePosition.Line,
                            linePosition.Character)));
                }
            }

            // Create a compilation result
            if(success && result.Success) {
                return CompilationResult.Successful(typ, messages);
            }
            return CompilationResult.Failed(messages);
        }

        private string MakeClassName(string fileName)
        {
            return "_" + InvalidClassNameChars.Replace(fileName, String.Empty);
        }
    }
}
