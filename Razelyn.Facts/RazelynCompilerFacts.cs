using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Razelyn.Facts
{
    public class RazelynCompilerFacts
    {
        [Fact]
        public void AdhocSuperCrazySpikeTest()
        {
            const string script = @"
@functions{ 
public string Output { get; set; } 
public void WriteLiteral(object lit) { Output += lit.ToString(); }
public void Write(object lit) { Output += lit.ToString(); }
}
@{Output = ""ZOMG IT WORKED"";}
";
            Type tmpl;
            using (StringReader reader = new StringReader(script))
            {
                tmpl = RazelynCompiler.Compile(reader);
            }

            dynamic instance = Activator.CreateInstance(tmpl);
            instance.Execute();
            Assert.Equal("ZOMG IT WORKED", ((string)instance.Output).Trim());
        }
    }
}
