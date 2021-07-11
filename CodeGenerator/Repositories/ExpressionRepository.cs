using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using CodeGenerator.Abstract.Entities.Expressions;
using CodeGenerator.Abstract.Repositories;
using static CodeGenerator.Roslyn.Consts.Spaces;
using static CodeGenerator.Roslyn.Consts.Spaces.Tabs;

namespace CodeGenerator.Roslyn.Repositories
{
    public class ExpressionRepository : IExpressionRepository
    {
        public TExpression AllignMethodsChaining<TExpression>(TExpression methodsInvocationExpressionEntity) where TExpression : MethodInvocationExpressionEntity
        {
            var methodsInvocationExpression = (InvocationExpressionSyntax)methodsInvocationExpressionEntity.ExpressionRoot;

            var diMethodAccessExpression = (MemberAccessExpressionSyntax)(methodsInvocationExpression.Expression);

            var newDiMethodAccessExpression = diMethodAccessExpression
                .WithOperatorToken(
                    SyntaxFactory.Token(
                        SyntaxFactory.TriviaList(new SyntaxTrivia[] { NEW_LINE }.Concat(FOUR_TABS)),
                        SyntaxKind.DotToken,
                        SyntaxFactory.TriviaList()
                    ));

            methodsInvocationExpression = methodsInvocationExpression.ReplaceNode(diMethodAccessExpression, newDiMethodAccessExpression);

            var isTopInvocation = methodsInvocationExpression
                .DescendantNodesAndSelf()
                .OfType<InvocationExpressionSyntax>()
                .Count()
                .Equals(1);

            if (!isTopInvocation)
            {
                var directInvocationChild = methodsInvocationExpression.DescendantNodes().OfType<InvocationExpressionSyntax>().First();
                var directInvocationChildEntity = new MethodInvocationExpressionEntity(directInvocationChild);

                var newDirectInvocationChild = AllignMethodsChaining(directInvocationChildEntity);
                methodsInvocationExpression = methodsInvocationExpression.ReplaceNode(directInvocationChild, (InvocationExpressionSyntax)newDirectInvocationChild.ExpressionRoot);
            }


            var newMethodsInvocationExpressionEntity = new MethodInvocationExpressionEntity(methodsInvocationExpression);
            return (TExpression)newMethodsInvocationExpressionEntity;
        }
    }
}