using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Web.Razor;
using System.Web.Razor.Parser;
using Microsoft.CSharp;
using Roslyn.Compilers;
using Roslyn.Compilers.CSharp;

namespace Razelyn
{
    /// <summary>
    /// Compiles Razor files in-memory using Roslyn
    /// </summary>
    public class RazelynCompiler
    {
        public static Type Compile(TextReader razorSource)
        {
            RazorTemplateEngine engine = new RazorTemplateEngine(new RazorEngineHost(new CSharpRazorCodeLanguage()) {
                DefaultNamespace = "RazelynTemp",
                DefaultClassName = "TheTemplate",
                DefaultBaseClass = "Razelyn.Template"
            });
            var results = engine.GenerateCode(razorSource);

            Assembly asm = Compile(results.GeneratedCode);
            return asm.GetType("RazelynTemp.TheTemplate");
        }

        public static Assembly Compile(CodeCompileUnit unit)
        {
            // Generate a string from the compilation unit
            CSharpCodeProvider provider = new CSharpCodeProvider();
            StringBuilder builder = new StringBuilder();
            using (StringWriter writer = new StringWriter(builder))
            {
                provider.GenerateCodeFromCompileUnit(unit, writer, new CodeGeneratorOptions());
            }

            // Parse into a syntax tree
            SyntaxTree tree = SyntaxTree.ParseCompilationUnit(builder.ToString());

            // Create a compilation
            Compilation comp = Compilation.Create("RazelynTemp.dll", new CompilationOptions(OutputKind.DynamicallyLinkedLibrary), new [] { tree }, new [] { 
                new AssemblyFileReference(typeof(object).Assembly.Location),
                new AssemblyFileReference(typeof(RazelynCompiler).Assembly.Location)});

            // Emit it to a new saveable dynamic assembly
            AssemblyBuilder asm = AssemblyBuilder.DefineDynamicAssembly(
                new AssemblyName("RazelynTemp"), AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder mod = asm.DefineDynamicModule("RazelynCompilation");
            ReflectionEmitResult result = comp.Emit(mod);
            if (!result.Success)
            {
                throw new InvalidOperationException("Failed to compile");
            }

            // Return the assembly
            return asm;
        }
    }
}
