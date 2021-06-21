using System.IO;

using Microsoft.CodeAnalysis.CSharp;

namespace CodeGenerator
{
    public class CodeFileReader
    {
        public CodeFileReader(string filePath)
        {
            using (var streamReader = new StreamReader(filePath))
            {
                var code = streamReader.ReadToEnd();
                var syntaxTree = CSharpSyntaxTree.ParseText(code);
                var compilationUnitSyntax = syntaxTree.GetCompilationUnitRoot();
            }
        }
    }
}
