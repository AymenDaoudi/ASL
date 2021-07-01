using System;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Domain.Entities.Expressions;
using Domain.AbstractRepositories.Expressions;

namespace CodeGenerator.Generators.Expressions
{
    public class MethodInvocationExpressionGenerator : IMethodInvocationExpressionGenerator
    {
        public IInitializedMethodInvocationExpressionGenerator Initialize(
            ExpressionEntityBase parentExpression, 
            string methodName, 
            params string[] typeArguments
        )
        {
            var method = SyntaxFactory.GenericName(
                SyntaxFactory.Identifier(methodName),
                SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList<TypeSyntax>(typeArguments.Select(ta => SyntaxFactory.IdentifierName(ta)))));

            var memberAccess = SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression, 
                (ExpressionSyntax)parentExpression.ExpressionRoot, 
                method
            );
            var argumentList = SyntaxFactory.SeparatedList(Array.Empty<ArgumentSyntax>());

            var methodInvocationExpression = SyntaxFactory.InvocationExpression(
                memberAccess,
                SyntaxFactory.ArgumentList(argumentList));

            var initializedMethodInvocationExpressionGenerator = new InitializedMethodInvocationExpressionGenerator(methodInvocationExpression);

            return initializedMethodInvocationExpressionGenerator;
        }

        private class InitializedMethodInvocationExpressionGenerator : ExpressionGeneratorBase<MethodInvocationExpressionEntity, InvocationExpressionSyntax>, IInitializedMethodInvocationExpressionGenerator
        {
            public InitializedMethodInvocationExpressionGenerator(InvocationExpressionSyntax invocationExpressionSyntax)
            {
                this._expression = invocationExpressionSyntax;
            }

            protected override MethodInvocationExpressionEntity GenerateExpressionEntity()
            {
                var methodInvocationExpressionEntity = new MethodInvocationExpressionEntity(_expression);

                return methodInvocationExpressionEntity;
            }
        }
    }
}