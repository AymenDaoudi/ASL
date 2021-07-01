using System.IO;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Domain.Entities.Namespaces;
using Domain.Entities.Types;
using Domain.AbstractRepositories.Files;

namespace CodeGenerator.Generators
{
    public class CodeFileGenerator : ICodeFileGenerator<TypeEntityBase>
    {
        public CodeFileGenerator()
        {

        }

        public void CreateFile(
            string filePath,
            NamespaceEntityBase<TypeEntityBase> namespaceEntity,
            params string[] usings
        )
        {
            var compilationUnit = SyntaxFactory.CompilationUnit(
                new SyntaxList<ExternAliasDirectiveSyntax>(),
                new SyntaxList<UsingDirectiveSyntax>(usings.Select(@using => SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(@using)))),
                new SyntaxList<AttributeListSyntax>(),
                new SyntaxList<MemberDeclarationSyntax>((NamespaceDeclarationSyntax)namespaceEntity.NamespaceRoot));

            using (FileStream fs = File.Create(filePath))
            {
                using (StreamWriter sourceWriter = new StreamWriter(fs))
                {
                    compilationUnit.NormalizeWhitespace().WriteTo(sourceWriter);
                }
            }
        }
    }
}