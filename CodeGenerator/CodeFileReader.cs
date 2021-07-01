using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Domain.AbstractRepositories.Files;
using Domain.Entities.Types.Classes;
using Domain.Entities.Files;

namespace CodeGenerator
{
    public class CodeFileReader : ICodeFileReader<CodeFileEntityBase>
    {
        public CodeFileReader()
        {
        }

        public async Task<CodeFileEntityBase> ReadAsync(string filePath)
        {
            CompilationUnitSyntax compilationUnitSyntax;

            using (var streamReader = new StreamReader(filePath))
            {
                var code = await streamReader.ReadToEndAsync();
                var syntaxTree = CSharpSyntaxTree.ParseText(code);
                compilationUnitSyntax = syntaxTree.GetCompilationUnitRoot();
            }

            var codeFileEntityBase = new CodeFileEntityBase(compilationUnitSyntax);

            return codeFileEntityBase;
        }

        public async Task<ClassEntityBase> ReadClassAsync(string filePath, string className)
        {
            var codeFile = await ReadAsync(filePath);

            var codeFileRoot = codeFile.CodeFileRoot as CompilationUnitSyntax;

            var classDeclarationSyntax = codeFileRoot
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .Single(c => c.Identifier.ValueText == className);

            var classEntity = new IServiceCollectionExtensionsEntity(className, classDeclarationSyntax);
            return classEntity;
        }
    }
}
