//using System;
//using System.CodeDom.Compiler;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;
//using System.Web.Razor;

//namespace Edge
//{
//    public class Compiler
//    {
//        private const string ClassName = "Page";

//        public static PageCompileResults Compile(IFileSystem fs, string path, TextWriter trace)
//        {
//            trace.WriteLine("\tCompiling: " + path);

//            // Setup the engine
//            RazorEngineHost host = ConstructHost();
//            RazorTemplateEngine engine = new RazorTemplateEngine(host);

//            // Compile the template
//            GeneratorResults genResults = null;
//            using (TextReader rdr = fs.OpenRead(path))
//            {
//                genResults = engine.GenerateCode(rdr);
//            }

//            List<string> errors = new List<string>();
//            if (!genResults.Success)
//            {
//                errors.AddRange(
//                    genResults.ParserErrors.Select(r => 
//                        String.Format(
//                            "[{0}]({1},{2}): {3}", 
//                            path, 
//                            r.Location.LineIndex, 
//                            r.Location.CharacterIndex, 
//                            r.Message)));
//            }

//            // TODO: Use the standard delegate-caching mechanism
//            fs.EnsureDirectory(".cache");
//            string asmName = String.Format("Edge_{0}_{1}", path.Replace('/', '_').Replace('.', '_'), Guid.NewGuid().ToString("N"));
//            CodeDomProvider provider = (CodeDomProvider)Activator.CreateInstance(host.CodeLanguage.CodeDomProviderType);
//            CompilerParameters parameters = new CompilerParameters();
//            parameters.ReferencedAssemblies.AddRange(
//                fs.GetAll("bin", "*.dll").ToArray());
//            foreach (string reference in parameters.ReferencedAssemblies)
//            {
//                trace.WriteLine("\t\tReferencing: " + reference);
//            }
//            parameters.OutputAssembly = @".cache\" + asmName;
//            trace.WriteLine("\t\tOutput: " + asmName);
//            CompilerResults compResults = provider.CompileAssemblyFromDom(parameters, genResults.GeneratedCode);

//            Page page = null;
//            if (compResults.Errors.HasErrors)
//            {
//                errors.AddRange(
//                    compResults.Errors.OfType<CompilerError>().Select(e =>
//                        String.Format(
//                            "[{0}]({1},{2}): {3}",
//                            e.FileName,
//                            e.Line,
//                            e.Column,
//                            e.ErrorText)));
//            }
//            else
//            {

//                // Pull the class out
//                Assembly asm = Assembly.LoadFrom(compResults.PathToAssembly);
//                if (asm == null)
//                {
//                    errors.Add(String.Format("[{0}] Unable to find page assembly in: {1}", path, compResults.PathToAssembly));
//                }
//                else
//                {
//                    Type pageType = asm.GetType(ClassName);
//                    if (pageType == null)
//                    {
//                        errors.Add(String.Format("[{0}] Unable to find page class in: {1}", path, asm.GetName().Name));
//                    }
//                    else
//                    {
//                        page = Activator.CreateInstance(pageType) as Page;
//                        if (page == null)
//                        {
//                            errors.Add(String.Format("[{0}] Page class does not derive from Edge.Page", path));
//                        }
//                    }
//                }
//            }

//            if (page == null)
//            {
//                StringWriter writer = new StringWriter();
//                provider.GenerateCodeFromCompileUnit(genResults.GeneratedCode, writer, new CodeGeneratorOptions());

//                return new PageCompileResults(writer.ToString(), errors);
//            }
//            else
//            {
//                return new PageCompileResults(page);
//            }
//        }

//        private static RazorEngineHost ConstructHost()
//        {
//            RazorEngineHost host = new RazorEngineHost(new CSharpRazorCodeLanguage());
//            host.DefaultClassName = ClassName;
//            host.DefaultNamespace = String.Empty;
//            host.DefaultBaseClass = typeof(Page).FullName;
//            return host;
//        }
//    }
//}
