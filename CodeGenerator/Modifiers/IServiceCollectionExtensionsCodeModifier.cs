using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using CodeGenerator.Exceptions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using static CodeGenerator.Consts.IServiceCollectionExtension;

namespace CodeGenerator.Modifiers
{
    public class IServiceCollectionExtensionsCodeModifier
    {
        private readonly SyntaxTriviaList _sixTabs = SyntaxFactory.TriviaList(
            SyntaxFactory.ElasticTab,
            SyntaxFactory.ElasticTab,
            SyntaxFactory.ElasticTab,
            SyntaxFactory.ElasticTab,
            SyntaxFactory.ElasticTab,
            SyntaxFactory.ElasticTab
        );

        private readonly SyntaxTriviaList _threeWhiteSpaces = SyntaxFactory.TriviaList(
            SyntaxFactory.ElasticWhitespace(" "),
            SyntaxFactory.ElasticWhitespace(" "),
            SyntaxFactory.ElasticWhitespace(" ")
        );

        private readonly SyntaxTrivia _newLineTrivia = SyntaxFactory.CarriageReturnLineFeed;

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
                        code.WriteTo(sourceWriter);
                    }
                }
            });
        }

        public Task RegisterNewRepositoryAsync(
            CompilationUnitSyntax code,
            string filePath,
            DILifetime dILifeTime,
            string abstractTypeName,
            string ImplementationTypeName
        )
        {
            MethodDeclarationSyntax registerRepositoriesMethod;

            try
            {
                registerRepositoriesMethod = code
                    .DescendantNodes()
                    .OfType<MethodDeclarationSyntax>()
                    .Single(m => m.Identifier.ValueText == REGISTER_REPOSITORIES);
            }
            catch (InvalidOperationException e) when (e.Message == "Sequence contains more than one matching element")
            {
                throw new NoOrMultipleRegisteServicesMethodsException(
                    NoOrMultipleRegisteServicesMethodsException.MULTIPLE_REGISTER_SERVICES_METHODS_ERROR_MESSAGE,
                    innerException: e
                );
            }
            catch (InvalidOperationException e) when (e.Message == "Sequence contains no matching element")
            {
                throw new NoOrMultipleRegisteServicesMethodsException(
                    NoOrMultipleRegisteServicesMethodsException.MULTIPLE_REGISTER_SERVICES_METHODS_ERROR_MESSAGE,
                    innerException: e
                );
            }

            var statements = new List<StatementSyntax>();

            var returnStatement = registerRepositoriesMethod
                .DescendantNodes()
                .OfType<ReturnStatementSyntax>()
                .SingleOrDefault();

            statements.Add(returnStatement);

            var lifeTimeDiMethodName = dILifeTime switch
            {
                DILifetime.Scoped => ADD_SCOPED,
                DILifetime.Transient => ADD_TRANSIENT,
                DILifetime.Singleton => ADD_SINGLETON,
                _ => ADD_SCOPED,
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

        public Task RegisterNewRepositoryAsync(
            CompilationUnitSyntax code,
            string filePath,
            DILifetime dILifeTime,
            string ImplementationTypeName
        )
        {
            MethodDeclarationSyntax registerRepositoriesMethod;

            try
            {
                registerRepositoriesMethod = code
                    .DescendantNodes()
                    .OfType<MethodDeclarationSyntax>()
                    .Single(m => m.Identifier.ValueText == REGISTER_REPOSITORIES);
            }
            catch (InvalidOperationException e) when (e.Message == "Sequence contains more than one matching element")
            {
                throw new NoOrMultipleRegisteServicesMethodsException(
                    NoOrMultipleRegisteServicesMethodsException.MULTIPLE_REGISTER_SERVICES_METHODS_ERROR_MESSAGE,
                    innerException: e
                );
            }
            catch (InvalidOperationException e) when (e.Message == "Sequence contains no matching element")
            {
                throw new NoOrMultipleRegisteServicesMethodsException(
                    NoOrMultipleRegisteServicesMethodsException.MULTIPLE_REGISTER_SERVICES_METHODS_ERROR_MESSAGE,
                    innerException: e
                );
            }

            var statements = new List<StatementSyntax>();

            var returnStatement = registerRepositoriesMethod
                .DescendantNodes()
                .OfType<ReturnStatementSyntax>()
                .First();

            statements.Add(returnStatement);

            var lifeTimeDiMethodName = dILifeTime switch
            {
                DILifetime.Scoped => ADD_SCOPED,
                DILifetime.Transient => ADD_TRANSIENT,
                DILifetime.Singleton => ADD_SINGLETON,
                _ => ADD_SCOPED,
            };

            var genericMethod = SyntaxFactory.GenericName(
                SyntaxFactory.Identifier(lifeTimeDiMethodName),
                SyntaxFactory.TypeArgumentList(
                    SyntaxFactory.SeparatedList<TypeSyntax>(new[]
                    {
                        SyntaxFactory.IdentifierName(ImplementationTypeName),
                    }
                )));


            var memberaccess = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, returnStatement.Expression, genericMethod);
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

        public Task RegisterNewServiceAsync(
            CompilationUnitSyntax code,
            string filePath,
            DILifetime dILifeTime,
            string abstractTypeName,
            string ImplementationTypeName
        )
        {
            MethodDeclarationSyntax registerServicesMethod;

            try
            {
                registerServicesMethod = code
                    .DescendantNodes()
                    .OfType<MethodDeclarationSyntax>()
                    .Single(m => m.Identifier.ValueText == REGISTER_SERVICES);
            }
            catch (InvalidOperationException e) when (e.Message == "Sequence contains more than one matching element")
            {
                throw new NoOrMultipleRegisteServicesMethodsException(
                    NoOrMultipleRegisteServicesMethodsException.MULTIPLE_REGISTER_SERVICES_METHODS_ERROR_MESSAGE, 
                    innerException: e
                );
            }
            catch (InvalidOperationException e) when (e.Message == "Sequence contains no matching element")
            {
                throw new NoOrMultipleRegisteServicesMethodsException(
                    NoOrMultipleRegisteServicesMethodsException.MULTIPLE_REGISTER_SERVICES_METHODS_ERROR_MESSAGE,
                    innerException: e
                );
            }

            var statements = new List<StatementSyntax>();

            var returnStatement = registerServicesMethod
                .DescendantNodes()
                .OfType<ReturnStatementSyntax>()
                .Single();

            statements.Add(returnStatement);

            var lifeTimeDiMethodName = dILifeTime switch
            {
                DILifetime.Scoped => ADD_SCOPED,
                DILifetime.Transient => ADD_TRANSIENT,
                DILifetime.Singleton => ADD_SINGLETON,
                _ => ADD_SCOPED,
            };

            var newLineTrivia = SyntaxFactory.SyntaxTrivia(SyntaxKind.EndOfLineTrivia, "\n");
            var newLineTrivia2 = SyntaxFactory.CarriageReturnLineFeed;
            SyntaxTriviaList threeLeadingTabs = SyntaxFactory.TriviaList(SyntaxFactory.ElasticTab, SyntaxFactory.ElasticTab, SyntaxFactory.ElasticTab);
            SyntaxTriviaList fourLeadingTabs = SyntaxFactory.TriviaList(SyntaxFactory.ElasticTab, SyntaxFactory.ElasticTab, SyntaxFactory.ElasticTab, SyntaxFactory.ElasticTab);
            SyntaxTriviaList leadingWhiteSpace = SyntaxFactory.TriviaList(SyntaxFactory.ElasticWhitespace(" "));

            var genericMethod = SyntaxFactory.GenericName(
                SyntaxFactory.Identifier(lifeTimeDiMethodName),
                SyntaxFactory.TypeArgumentList(
                    SyntaxFactory.SeparatedList<TypeSyntax>(new[]
                    {
                        SyntaxFactory.IdentifierName(abstractTypeName),
                        SyntaxFactory.IdentifierName(ImplementationTypeName),
                    }
                )));

            var memberAccess = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, returnStatement.Expression, genericMethod);

            var argumentList = SyntaxFactory.SeparatedList(Array.Empty<ArgumentSyntax>());

            var newReturnStatement = SyntaxFactory
                .ReturnStatement(SyntaxFactory.InvocationExpression(memberAccess, SyntaxFactory.ArgumentList(argumentList)))
                .NormalizeWhitespace()
                .WithLeadingTrivia(threeLeadingTabs)
                .WithTrailingTrivia(newLineTrivia2);

            var invocation = newReturnStatement.DescendantNodes().OfType<InvocationExpressionSyntax>().First();

            var newInvocation = breakLineForInvocations(invocation);

            newReturnStatement = newReturnStatement.ReplaceNode(invocation, newInvocation);

            code = code.TrackNodes(statements.Distinct());
            foreach (var statement in statements)
            {
                var g = code.GetCurrentNode(statement);
                code = code.ReplaceNode(g, newReturnStatement);
            }

            return SaveAsync(code, filePath);
        }

        public Task RegisterNewServiceAsync(
            CompilationUnitSyntax code,
            string filePath,
            DILifetime dILifeTime,
            string ImplementationTypeName
        )
        {
            MethodDeclarationSyntax registerServicesMethod;

            try
            {
                registerServicesMethod = code
                    .DescendantNodes()
                    .OfType<MethodDeclarationSyntax>()
                    .Single(m => m.Identifier.ValueText == REGISTER_SERVICES);
            }
            catch (InvalidOperationException e) when (e.Message == "Sequence contains more than one matching element")
            {
                throw new NoOrMultipleRegisteServicesMethodsException(
                    NoOrMultipleRegisteServicesMethodsException.MULTIPLE_REGISTER_SERVICES_METHODS_ERROR_MESSAGE,
                    innerException: e
                );
            }
            catch (InvalidOperationException e) when (e.Message == "Sequence contains no matching element")
            {
                throw new NoOrMultipleRegisteServicesMethodsException(
                    NoOrMultipleRegisteServicesMethodsException.MULTIPLE_REGISTER_SERVICES_METHODS_ERROR_MESSAGE,
                    innerException: e
                );
            }

            var statements = new List<StatementSyntax>();

            var returnStatement = registerServicesMethod
                .DescendantNodes()
                .OfType<ReturnStatementSyntax>()
                .First();

            statements.Add(returnStatement);

            var lifeTimeDiMethodName = dILifeTime switch
            {
                DILifetime.Scoped => ADD_SCOPED,
                DILifetime.Transient => ADD_TRANSIENT,
                DILifetime.Singleton => ADD_SINGLETON,
                _ => ADD_SCOPED,
            };

            var genericMethod = SyntaxFactory.GenericName(
                SyntaxFactory.Identifier(lifeTimeDiMethodName),
                SyntaxFactory.TypeArgumentList(
                    SyntaxFactory.SeparatedList<TypeSyntax>(new[]
                    {
                        SyntaxFactory.IdentifierName(ImplementationTypeName),
                    }
                )));


            var memberaccess = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, returnStatement.Expression, genericMethod);
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

        private InvocationExpressionSyntax breakLineForInvocations(InvocationExpressionSyntax diMethodInvocationExpression)
        {
            var hasInvocations = diMethodInvocationExpression.DescendantNodes().Any(i => i is InvocationExpressionSyntax);

            if (!hasInvocations)
            {
                var newDiMethodInvocationExpression = diMethodInvocationExpression
                    .NormalizeWhitespace()
                    .WithTrailingTrivia(_newLineTrivia);

                return newDiMethodInvocationExpression;
            }
            else
            {
                var isTopInvocation = diMethodInvocationExpression.Parent is ReturnStatementSyntax;
                var diMethodAccessExpression = (MemberAccessExpressionSyntax)diMethodInvocationExpression.Expression;
                var newDiMethodAccessExpression = diMethodAccessExpression
                    .WithOperatorToken(
                        SyntaxFactory.Token(
                            SyntaxFactory.TriviaList(_sixTabs.Concat(_threeWhiteSpaces.AsEnumerable())),
                            SyntaxKind.DotToken,
                            SyntaxFactory.TriviaList()
                        ));
                diMethodInvocationExpression = diMethodInvocationExpression.ReplaceNode(diMethodAccessExpression, newDiMethodAccessExpression);
                var directInvocationChild = diMethodInvocationExpression.DescendantNodes().OfType<InvocationExpressionSyntax>().First();
                var newDirectInvocationChild = breakLineForInvocations(directInvocationChild);

                diMethodInvocationExpression = diMethodInvocationExpression.ReplaceNode(directInvocationChild, newDirectInvocationChild);

                if (!isTopInvocation)
                {
                    diMethodInvocationExpression = diMethodInvocationExpression.WithTrailingTrivia(_newLineTrivia);
                }

                return diMethodInvocationExpression;
            }
        }
    }
}
