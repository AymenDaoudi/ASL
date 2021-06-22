using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeGenerator
{
    public class CodeFileReader
    {
        public CodeFileReader()
        {
        }

        public virtual async Task<CompilationUnitSyntax> ReadAsync(string filePath)
        {
            CompilationUnitSyntax compilationUnitSyntax;

            using (var streamReader = new StreamReader(filePath))
            {
                var code = await streamReader.ReadToEndAsync();
                var syntaxTree = CSharpSyntaxTree.ParseText(code);
                compilationUnitSyntax = syntaxTree.GetCompilationUnitRoot();
            }

            return compilationUnitSyntax;
        }
    }
}
