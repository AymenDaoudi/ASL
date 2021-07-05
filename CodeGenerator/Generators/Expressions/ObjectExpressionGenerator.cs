using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CodeGenerator.Abstract.Generators.Expressions;
using CodeGenerator.Abstract.Entities.Expressions;

namespace CodeGenerator.Roslyn.Generators.Expressions
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