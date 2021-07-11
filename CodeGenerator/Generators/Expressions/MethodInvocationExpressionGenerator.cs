using System;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using CodeGenerator.Abstract.Entities.Expressions;
using CodeGenerator.Abstract.Generators.Expressions;
using CodeGenerator.Abstract.Repositories;

namespace CodeGenerator.Roslyn.Generators.Expressions
{
    public class MethodInvocationExpressionGenerator : IMethodInvocationExpressionGenerator
    {

        private readonly IExpressionRepository _expressionRepository;

        public MethodInvocationExpressionGenerator(IExpressionRepository expressionRepository)
        {
            _expressionRepository = expressionRepository;
        }

        public IInitializedMethodInvocationExpressionGenerator Initialize(
            ExpressionEntityBase parentExpression,
            string methodName,
            params string[] typeArguments
        )
        {
            SimpleNameSyntax method = null;

            if (typeArguments?.Any() == true)
            {
                method = SyntaxFactory.GenericName(
                    SyntaxFactory.Identifier(methodName),
                    SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList<TypeSyntax>(typeArguments.Select(ta => SyntaxFactory.IdentifierName(ta)))));
            }
            else
            {
                method = SyntaxFactory.IdentifierName(methodName);
            }

            var memberAccess = SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                (ExpressionSyntax)parentExpression.ExpressionRoot,
                method
            );

            var argumentList = SyntaxFactory.SeparatedList(Array.Empty<ArgumentSyntax>());

            var methodInvocationExpression = SyntaxFactory.InvocationExpression(
                memberAccess,
                SyntaxFactory.ArgumentList(argumentList));

            var initializedMethodInvocationExpressionGenerator = new InitializedMethodInvocationExpressionGenerator(methodInvocationExpression, _expressionRepository);

            return initializedMethodInvocationExpressionGenerator;
        }

        private class InitializedMethodInvocationExpressionGenerator : ExpressionGeneratorBase<MethodInvocationExpressionEntity, InvocationExpressionSyntax>, IInitializedMethodInvocationExpressionGenerator
        {
            private readonly IExpressionRepository _expressionRepository;

            public InitializedMethodInvocationExpressionGenerator(InvocationExpressionSyntax invocationExpressionSyntax, IExpressionRepository expressionRepository)
            {
                _expression = invocationExpressionSyntax;
                _expressionRepository = expressionRepository;
            }

            public IInitializedMethodInvocationExpressionGenerator ChainMethodInvocation(string methodName, params string[] typeArguments)
            {
                SimpleNameSyntax method = null;

                if (typeArguments?.Any() == true)
                {
                    method = SyntaxFactory.GenericName(
                        SyntaxFactory.Identifier(methodName),
                        SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList<TypeSyntax>(typeArguments.Select(ta => SyntaxFactory.IdentifierName(ta)))));
                }
                else
                {
                    method = SyntaxFactory.IdentifierName(methodName);
                }

                var memberAccess = SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    _expression,
                    method
                );

                var argumentList = SyntaxFactory.SeparatedList(Array.Empty<ArgumentSyntax>());

                _expression = SyntaxFactory.InvocationExpression(
                    memberAccess,
                    SyntaxFactory.ArgumentList(argumentList));

                var invocationEntity = new MethodInvocationExpressionEntity(_expression);
                var newInvocation = _expressionRepository.AllignMethodsChaining(invocationEntity);

                _expression = (InvocationExpressionSyntax)newInvocation.ExpressionRoot;

                return this;
            }

            protected override MethodInvocationExpressionEntity GenerateExpressionEntity()
            {
                var methodInvocationExpressionEntity = new MethodInvocationExpressionEntity(_expression);

                return methodInvocationExpressionEntity;
            }
        }
    }
}