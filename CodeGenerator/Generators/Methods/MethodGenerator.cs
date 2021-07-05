using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CodeGenerator.Abstract.Generators.Modifiers;
using CodeGenerator.Abstract.Generators.Methods;
using CodeGenerator.Abstract.Entities.Methods;
using CodeGenerator.Abstract.Entities.Statements;
using CodeGenerator.Abstract.Entities;

namespace CodeGenerator.Roslyn.Generators.Methods
{
    public class MethodGenerator : IInstanceMethodGenerator<MethodEntityBase, StatementEntityBase, ParameterEntityBase>
    {
        private readonly IAccessModifierMapper<SyntaxToken> _accessModifierMapper;

        public MethodGenerator(IAccessModifierMapper<SyntaxToken> accessModifierMapper)
        {
            _accessModifierMapper = accessModifierMapper;
        }

        public IInitializedMethodGenerator<MethodEntityBase, StatementEntityBase, ParameterEntityBase> Initialize(
            string methodName,
            string returnTypeName,
            AccessModifiers modifiers
        )
        {
            var method = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName(returnTypeName), methodName);
            method = method.AddModifiers(_accessModifierMapper.From(modifiers));

            var initializedMethodGenerator = new InitializedMethodGenerator(_accessModifierMapper, method);

            return initializedMethodGenerator;
        }

        private class InitializedMethodGenerator : MethodGeneratorBase<MethodEntityBase, MethodDeclarationSyntax>, IInitializedInstanceMethodGenerator<MethodEntityBase, StatementEntityBase, ParameterEntityBase>
        {
            private readonly IAccessModifierMapper<SyntaxToken> _accessModifierMapper;

            public InitializedMethodGenerator(
                IAccessModifierMapper<SyntaxToken> accessModifierMapper,
                MethodDeclarationSyntax method
            )
            {
                _method = method;
                _accessModifierMapper = accessModifierMapper;
            }

            public IInitializedMethodGenerator<MethodEntityBase, StatementEntityBase, ParameterEntityBase> SetParameters(params ParameterEntityBase[] parameters)
            {
                var parameterSynatxes = parameters.Select(parameter => SyntaxFactory.Parameter(
                    new SyntaxList<AttributeListSyntax>(),
                    new SyntaxTokenList(),
                    SyntaxFactory.ParseTypeName(parameter.ParameterTypeName),
                    SyntaxFactory.Identifier(parameter.ParameterName),
                    null
                ));

                _method = _method.AddParameterListParameters(parameterSynatxes.ToArray());

                return this;
            }

            public IInitializedMethodGenerator<MethodEntityBase, StatementEntityBase, ParameterEntityBase> SetStatements(params StatementEntityBase[] statements)
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

            protected override MethodEntityBase GenerateMethodEntity()
            {
                var methodEntity = new MethodEntityBase(
                    _method,
                    _method.Identifier.ValueText,
                    _accessModifierMapper.To(_method.Modifiers.ToArray())
                );

                return methodEntity;
            }
        }
    }
}