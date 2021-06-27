﻿using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using CodeGenerator.Generators.Mappers;
using Domain.Entities.Methods;
using Domain.Entities.Statements;
using Domain.AbstractRepositories.Methods;

namespace CodeGenerator.Generators.Methods
{
    public class ExtensionMethodGenerator : MethodGeneratorBase<ExtensionMethodEntity, MethodDeclarationSyntax>, IMethodGenerator<ExtensionMethodEntity, StatementEntityBase, ParameterEntityBase>
    {
        public ExtensionMethodGenerator Initialize(
            string methodName, 
            string returnTypeName, 
            string extendedTypeName, 
            string extendedTypeParameterName
        )
        {
            var accessModifiersMapper = new AccessModifiersMapper();

            _method = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName(returnTypeName), methodName);
            _method = _method.AddModifiers(accessModifiersMapper.From(ExtensionMethodEntity.Modifiers));
            
            var ExtendedParam = SyntaxFactory.Parameter(
                new SyntaxList<AttributeListSyntax>(),
                new SyntaxTokenList(SyntaxFactory.Token(SyntaxKind.ThisKeyword)),
                SyntaxFactory.ParseTypeName(extendedTypeName),
                SyntaxFactory.Identifier(extendedTypeParameterName),
                null
            );
            _method = _method.AddParameterListParameters(ExtendedParam);

            return this;
        }

        public IMethodGenerator<ExtensionMethodEntity, StatementEntityBase, ParameterEntityBase> SetParameters(params ParameterEntityBase[] parameters)
        {
            throw new System.NotImplementedException();
        }

        public IMethodGenerator<ExtensionMethodEntity, StatementEntityBase, ParameterEntityBase> SetStatements(params StatementEntityBase[] statements)
        {
            //Todo: To Convert to a strategy pattern
            var statementSyntaxes = new List<StatementSyntax>();

            foreach (var statement in statements)
            {
                if (statement is ReturnStatementEntity)
                {
                    statementSyntaxes.Add(SyntaxFactory.ReturnStatement(SyntaxFactory.IdentifierName(((ReturnStatementEntity)statement).Expression)));
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
