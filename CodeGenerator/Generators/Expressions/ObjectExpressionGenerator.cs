using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Domain.Entities.Expressions;
using Domain.AbstractRepositories.Expressions;

namespace CodeGenerator.Generators.Expressions
{
    public class ObjectExpressionGenerator : IObjectExpressionGenerator
    {
        public IInitializedObjectExpressionGenerator Initialize(string objectName)
        {
            var expressionRoot = SyntaxFactory.IdentifierName(objectName);

            var initializedObjectExpressionGenerator = new InitializedObjectExpressionGenerator(expressionRoot);

            return initializedObjectExpressionGenerator;
        }

        private class InitializedObjectExpressionGenerator : ExpressionGeneratorBase<ObjectExpressionEntity, IdentifierNameSyntax>, IInitializedObjectExpressionGenerator
        {
            public InitializedObjectExpressionGenerator(IdentifierNameSyntax expression)
            {
                _expression = expression;
            }

            protected override ObjectExpressionEntity GenerateExpressionEntity()
            {
                var objectExpressionEntity = new ObjectExpressionEntity(_expression, _expression.Identifier.ValueText);

                return objectExpressionEntity;
            }
        }
    }
}