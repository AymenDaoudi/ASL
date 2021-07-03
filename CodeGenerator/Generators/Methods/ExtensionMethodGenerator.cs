using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Domain.Entities.Methods;
using Domain.Entities.Statements;
using Domain.AbstractRepositories.Methods;
using Domain.AbstractRepositories.Modifiers;

namespace CodeGenerator.Generators.Methods
{
    public class ExtensionMethodGenerator : IExtensionMethodGenerator<ExtensionMethodEntity, StatementEntityBase, ParameterEntityBase>
    {
        private readonly IAccessModifierMapper<SyntaxToken> _accessModifierMapper;

        public ExtensionMethodGenerator(IAccessModifierMapper<SyntaxToken> accessModifierMapper)
        {
            _accessModifierMapper = accessModifierMapper;
        }

        public IInitializedMethodGenerator<ExtensionMethodEntity, StatementEntityBase, ParameterEntityBase> Initialize(
            string methodName,
            string returnTypeName,
            string extendedTypeName,
            string extendedTypeParameterName
        )
        {
            var method = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName(returnTypeName), methodName);
            method = method.AddModifiers(_accessModifierMapper.From(ExtensionMethodEntity.Modifiers));
            
            var ExtendedParam = SyntaxFactory.Parameter(
                new SyntaxList<AttributeListSyntax>(),
                new SyntaxTokenList(SyntaxFactory.Token(SyntaxKind.ThisKeyword)),
                SyntaxFactory.ParseTypeName(extendedTypeName),
                SyntaxFactory.Identifier(extendedTypeParameterName),
                null
            );

            method = method.AddParameterListParameters(ExtendedParam);

            var initializedExtensionMethodGenerator = new InitializedExtensionMethodGenerator(method);

            return initializedExtensionMethodGenerator;
        }

        private class InitializedExtensionMethodGenerator : MethodGeneratorBase<ExtensionMethodEntity, MethodDeclarationSyntax>, IInitializedExtensionMethodGenerator<ExtensionMethodEntity, StatementEntityBase, ParameterEntityBase>
        {
            public InitializedExtensionMethodGenerator(MethodDeclarationSyntax method)
            {
                this._method = method;
            }

            public IInitializedMethodGenerator<ExtensionMethodEntity, StatementEntityBase, ParameterEntityBase> SetParameters(params ParameterEntityBase[] parameters)
            {
                throw new System.NotImplementedException();
            }

            public IInitializedMethodGenerator<ExtensionMethodEntity, StatementEntityBase, ParameterEntityBase> SetStatements(params StatementEntityBase[] statements)
            {
                //Todo: To Convert to a strategy pattern
                var statementSyntaxes = new List<StatementSyntax>();

                foreach (var statement in statements)
                {
                    if (statement is ReturnStatementEntity)
                    {
                        statementSyntaxes.Add(SyntaxFactory.ReturnStatement((ExpressionSyntax)((ReturnStatementEntity)statement).Expression.ExpressionRoot));
                    }
                }

                _method = _method.AddBodyStatements(statementSyntaxes.ToArray());

                return this;
            }

            protected override ExtensionMethodEntity GenerateMethodEntity()
            {
                var classEntity = new ExtensionMethodEntity(
                    _method,
                    _method.Identifier.ValueText,
                    _method.ParameterList.Parameters.First().Type.ToString(),
                    _method.ParameterList.Parameters.First().Identifier.ValueText
                );

                return classEntity;
            }
        }
    }
}
