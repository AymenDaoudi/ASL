using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Domain.Entities.Statements;
using Domain.AbstractRepositories.Files;
using Domain.Entities.Files;

namespace CodeGenerator.Generators.Classes
{
    public class CodeFileModifier : ICodeFileModifier
    {
        private readonly SyntaxTrivia _newLineTrivia = SyntaxFactory.CarriageReturnLineFeed;
        private readonly SyntaxTriviaList _threeLeadingTabs = SyntaxFactory.TriviaList(SyntaxFactory.ElasticTab, SyntaxFactory.ElasticTab, SyntaxFactory.ElasticTab);

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

        private readonly ICodeFileReader<CodeFileEntityBase> _codeFileReader;

        public CodeFileModifier(ICodeFileReader<CodeFileEntityBase> codeFileReader)
        {
            _codeFileReader = codeFileReader;
        }

        public async Task ReplaceReturnStatementOfMethodOfClassAsync(
            string filePath, 
            string className, 
            string methodName, 
            ReturnStatementEntity newRturnStatement
        )
        {
            var codeFile = await _codeFileReader.ReadAsync(filePath);
            var codeFileRoot = codeFile.CodeFileRoot as CompilationUnitSyntax;

            var methodDeclarationSyntax = codeFileRoot
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .Single(c => c.Identifier.ValueText == className)
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .Single(m => m.Identifier.ValueText == methodName);

            var statements = new List<StatementSyntax>();

            var returnStatement = methodDeclarationSyntax
                .DescendantNodes()
                .OfType<ReturnStatementSyntax>()
                .SingleOrDefault();

            statements.Add(returnStatement);

            var newReturnStatement = SyntaxFactory
                .ReturnStatement((ExpressionSyntax)newRturnStatement.Expression.ExpressionRoot)
                .NormalizeWhitespace()
                .WithLeadingTrivia(_threeLeadingTabs)
                .WithTrailingTrivia(_newLineTrivia);

            var invocation = newReturnStatement
                .DescendantNodes()
                .OfType<InvocationExpressionSyntax>()
                .First();

            var newInvocation = BreakLineForInvocations(invocation);

            newReturnStatement = newReturnStatement.ReplaceNode(invocation, newInvocation);

            codeFileRoot = codeFileRoot.TrackNodes(statements.Distinct());
            foreach (var statement in statements)
            {
                var currentStatement = codeFileRoot.GetCurrentNode(statement);
                codeFileRoot = codeFileRoot.ReplaceNode(currentStatement, newReturnStatement);
            }

            await SaveAsync(codeFileRoot, filePath);
        }

        private Task SaveAsync(CompilationUnitSyntax code, string filePath)
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

        private InvocationExpressionSyntax BreakLineForInvocations(InvocationExpressionSyntax diMethodInvocationExpression)
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
                var newDirectInvocationChild = BreakLineForInvocations(directInvocationChild);

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