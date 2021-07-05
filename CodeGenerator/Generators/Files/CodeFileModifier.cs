using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using CodeGenerator.Abstract.Generators.Files;
using CodeGenerator.Abstract.Entities.Files;
using CodeGenerator.Abstract.Entities.Statements;
using static CodeGenerator.Roslyn.Consts.Spaces;
using static CodeGenerator.Roslyn.Consts.Spaces.Tabs;
using static CodeGenerator.Roslyn.Consts.Spaces.Whitespaces;

namespace CodeGenerator.Roslyn.Generators.Files
{
    public class CodeFileModifier : ICodeFileModifier
    {
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
                .WithLeadingTrivia(THREE_TABS)
                .WithTrailingTrivia(NEW_LINE);

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
            var hasInvocations = diMethodInvocationExpression
                .DescendantNodes()
                .Any(i => i is InvocationExpressionSyntax);

            if (!hasInvocations)
            {
                var newDiMethodInvocationExpression = diMethodInvocationExpression.NormalizeWhitespace();

                return newDiMethodInvocationExpression;
            }
            else
            {
                var isTopInvocation = diMethodInvocationExpression.Parent is ReturnStatementSyntax;
                var diMethodAccessExpression = (MemberAccessExpressionSyntax)diMethodInvocationExpression.Expression;
                var newDiMethodAccessExpression = diMethodAccessExpression
                    .WithOperatorToken(
                        SyntaxFactory.Token(
                            SyntaxFactory.TriviaList(new SyntaxTrivia[] { NEW_LINE }.Concat(SIX_TABS.Concat(THREE_WHITE_SPACES))),
                            SyntaxKind.DotToken,
                            SyntaxFactory.TriviaList()
                        ));

                diMethodInvocationExpression = diMethodInvocationExpression.ReplaceNode(diMethodAccessExpression, newDiMethodAccessExpression);
                var directInvocationChild = diMethodInvocationExpression.DescendantNodes().OfType<InvocationExpressionSyntax>().First();
                var newDirectInvocationChild = BreakLineForInvocations(directInvocationChild);

                diMethodInvocationExpression = diMethodInvocationExpression.ReplaceNode(directInvocationChild, newDirectInvocationChild);

                return diMethodInvocationExpression;
            }
        }
    }
}