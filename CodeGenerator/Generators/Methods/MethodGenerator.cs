using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using CodeGenerator.Generators.Mappers;
using Domain.AbstractRepositories.Methods;
using Domain.Entities;
using Domain.Entities.Methods;
using Domain.Entities.Statements;

namespace CodeGenerator.Generators.Methods
{
    public class MethodGenerator : IInstanceMethodGenerator<MethodEntityBase, StatementEntityBase, ParameterEntityBase>
    {
        public IInitializedMethodGenerator<MethodEntityBase, StatementEntityBase, ParameterEntityBase> Initialize(
            string methodName, 
            string returnTypeName, 
            Modifiers modifiers
        )
        {
            var accessModifiersMapper = new AccessModifiersMapper();
            
            var method = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName(returnTypeName), methodName);
            method = method.AddModifiers(accessModifiersMapper.From(modifiers));

            var initializedMethodGenerator = new InitializedMethodGenerator(method);
            
            return initializedMethodGenerator;
        }

        private class InitializedMethodGenerator : MethodGeneratorBase<MethodEntityBase, MethodDeclarationSyntax>, IInitializedInstanceMethodGenerator<MethodEntityBase, StatementEntityBase, ParameterEntityBase>
        {
            public InitializedMethodGenerator(MethodDeclarationSyntax method)
            {
                this._method = method;
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
                AccessModifiersMapper accessModifiersMapper = new AccessModifiersMapper();

                var methodEntity = new MethodEntityBase(
                    _method,
                    _method.Identifier.ValueText,
                    accessModifiersMapper.To(_method.Modifiers.ToArray())
                );

                return methodEntity;
            }
        }
    }
}