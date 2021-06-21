using System;
using System.Collections.Generic;
using System.Linq;

using CodeGenerator.Entities;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeGenerator.Generators.Methods
{
    public class ExtensionMethodGenerator : MethodAbstractGenerator<ExtensionMethodEntity>
    {
        public override ExtensionMethodGenerator Initialize(ExtensionMethodEntity methodCode, string returnTypeName)
        {
            var extensionMethodGenerator = (ExtensionMethodGenerator)base.Initialize(methodCode, returnTypeName);
            SetExtendedType(methodCode.ExtendedTypeName, methodCode.ExtendedTypeParameterName);

            return extensionMethodGenerator;
        }

        private void SetExtendedType(string extendedTypeName, string extendedTypeParameterName)
        {
            var ExtendedParam = SyntaxFactory.Parameter(
                new SyntaxList<AttributeListSyntax>(), 
                new SyntaxTokenList(SyntaxFactory.Token(SyntaxKind.ThisKeyword)), 
                SyntaxFactory.ParseTypeName(extendedTypeName), 
                SyntaxFactory.Identifier(extendedTypeParameterName), 
                null
            );

            method = method.AddParameterListParameters(ExtendedParam);
        }

        public override ExtensionMethodGenerator SetParameters(IEnumerable<(Type type, string parameterName)> parameters)
        {
            var parameterSynatxes = parameters.Select(parameter => SyntaxFactory.Parameter(
                new SyntaxList<AttributeListSyntax>(),
                new SyntaxTokenList(),
                SyntaxFactory.ParseTypeName(nameof(parameter.type)),
                SyntaxFactory.Identifier(parameter.parameterName),
                null
            ));

            method = method.AddParameterListParameters(parameterSynatxes.ToArray());

            return this;
        }

        public override ExtensionMethodGenerator SetStatements(params StatementSyntax[] statements)
        {
            method = method.AddBodyStatements(statements);

            return this;
        }
    }
}
