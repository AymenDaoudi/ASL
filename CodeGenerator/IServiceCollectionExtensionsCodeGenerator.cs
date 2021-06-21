using System.Linq;
using System.Collections.Generic;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using CodeGenerator.Generators.Methods;
using CodeGenerator.Entities;
using CodeGenerator.Generators.Classes;

namespace CodeGenerator
{
    public class IServiceCollectionExtensionsCodeGenerator : CodeFileAbstractGenerator
    {
        public IServiceCollectionExtensionsCodeGenerator(
            string @namespace,
            params string[] usings
        ) : base(@namespace, usings)
        {
        }

        private IEnumerable<MethodDeclarationSyntax> GenerateMethods()
        {
            var extensionMethodGenerator = new ExtensionMethodGenerator();

            var registerRepositoriesMethod = extensionMethodGenerator
                .Initialize(new ExtensionMethodEntity("RegisterRepositories", nameof(IServiceCollection), "services"), nameof(IServiceCollection))
                .SetStatements(SyntaxFactory.ReturnStatement(SyntaxFactory.IdentifierName("services")))
                .Generate();

            var registerServicesMethod = extensionMethodGenerator
                .Initialize(new ExtensionMethodEntity("RegisterServices", nameof(IServiceCollection), "services"), nameof(IServiceCollection))
                .SetStatements(SyntaxFactory.ReturnStatement(SyntaxFactory.IdentifierName("services")))
                .Generate();

            return new MethodDeclarationSyntax[]{registerRepositoriesMethod, registerServicesMethod};
        }

        protected override CompilationUnitSyntax GenerateCode()
        {
            var classGenerator = new ClassGenerator();
            var modifiers = new SyntaxToken[]
            {
                SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                SyntaxFactory.Token(SyntaxKind.StaticKeyword)
            };

            var @class = classGenerator
                .Initialize("IServiceCollectionExtensions", modifiers)
                .SetMethods(GenerateMethods().ToArray())
                .Generate();
            
            var namespaceDeclaration = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(@namespace));
            namespaceDeclaration = namespaceDeclaration
                .AddMembers(@class);

            var compilationUnit = SyntaxFactory.CompilationUnit(
                new SyntaxList<ExternAliasDirectiveSyntax>(),
                new SyntaxList<UsingDirectiveSyntax>(usings.Select(@using => SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(@using)))),
                new SyntaxList<AttributeListSyntax>(),
                new SyntaxList<MemberDeclarationSyntax>(namespaceDeclaration));

            return compilationUnit;
        }
    }
}
