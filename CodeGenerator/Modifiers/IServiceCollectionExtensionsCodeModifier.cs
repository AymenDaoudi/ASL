using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;

namespace CodeGenerator.Modifiers
{
    public class IServiceCollectionExtensionsCodeModifier
    {
        public IServiceCollectionExtensionsCodeModifier()
        {

        }

        public Task SaveAsync(CompilationUnitSyntax code, string filePath)
        {
            return Task.Run(() =>
            {
                using (FileStream fs = File.Create(filePath))
                {
                    using (StreamWriter sourceWriter = new StreamWriter(fs))
                    {
                        code.NormalizeWhitespace().WriteTo(sourceWriter);
                    }
                }
            });
        }

        public Task RegisterNewRepositoryAsync(
            CompilationUnitSyntax code,
            string filePath,
            DILifeTime dILifeTime, 
            string abstractTypeName, 
            string ImplementationTypeName
        )
        {
            var namespaceSyntax = code.Members.OfType<NamespaceDeclarationSyntax>().First();
            var classSyntax = namespaceSyntax.Members.OfType<ClassDeclarationSyntax>().First();
            var registerRepositoriesMethod = classSyntax.Members.OfType<MethodDeclarationSyntax>().First();

            var statements = new List<StatementSyntax>();

            var returnStatement = registerRepositoriesMethod
                .DescendantNodes()
                .OfType<ReturnStatementSyntax>()
                .First();
            statements.Add(returnStatement);

            var lifeTimeDiMethodName = dILifeTime switch
            {
                DILifeTime.Scoped => "AddScoped",
                DILifeTime.Transient => "AddTransient",
                DILifeTime.Singleton => "AddSingleton",
                _ => "AddScoped",
            };

            var genericMethod = SyntaxFactory.GenericName(
                SyntaxFactory.Identifier(lifeTimeDiMethodName),
                SyntaxFactory.TypeArgumentList(
                    SyntaxFactory.SeparatedList<TypeSyntax>(new[]
                    {
                        SyntaxFactory.IdentifierName(abstractTypeName),
                        SyntaxFactory.IdentifierName(ImplementationTypeName),
                    }
                )));
            

            var memberaccess = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, returnStatement.Expression, genericMethod);

            var argument = SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal("A")));
            var argumentList = SyntaxFactory.SeparatedList(Array.Empty<ArgumentSyntax>());

            var newReturnStatement =
                SyntaxFactory.ReturnStatement(
                SyntaxFactory.InvocationExpression(memberaccess,
                SyntaxFactory.ArgumentList(argumentList)));

            code = code.TrackNodes(statements.Distinct());
            foreach (var statement in statements)
            {
                var g = code.GetCurrentNode(statement);
                code = code.ReplaceNode(g, newReturnStatement);
            }

            return SaveAsync(code, filePath);
        }

        public void RegisterNewRepository(
            CompilationUnitSyntax code, 
            DILifeTime dILifeTime, 
            string ImplementationTypeName
        )
        {

        }

        public void RegisterNewService(
            CompilationUnitSyntax code,
            DILifeTime dILifeTime,
            string abstractTypeName,
            string ImplementationTypeName
        )
        {

        }

        public void RegisterNewService(
            CompilationUnitSyntax code, 
            DILifeTime dILifeTime, 
            string ImplementationTypeName
        )
        {

        }
    }
}
