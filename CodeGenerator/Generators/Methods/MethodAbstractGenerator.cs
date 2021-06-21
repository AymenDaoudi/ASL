using System;
using System.Collections.Generic;

using CodeGenerator.Entities;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeGenerator.Generators.Methods
{
    public abstract class MethodAbstractGenerator<TMethodCode> where TMethodCode : MethodEntityBase, new()
    {
        protected MethodDeclarationSyntax method;

        public TMethodCode MethodCode { get; private set; }

        public virtual MethodAbstractGenerator<TMethodCode> Initialize(TMethodCode methodCode, string returnTypeName)
        {
            method = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName(returnTypeName), methodCode.MethodName);
            method = method.AddModifiers(methodCode.Modifiers);
            return this;
        }

        public abstract MethodAbstractGenerator<TMethodCode> SetParameters(IEnumerable<(Type type, string parameterName)> parameters);

        public abstract MethodAbstractGenerator<TMethodCode> SetStatements(params StatementSyntax[] statements);

        public virtual MethodDeclarationSyntax Generate()
        {
            var generatedMethod = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName("void"), string.Empty);
            generatedMethod = method;

            Reset();

            return generatedMethod;
        }

        public void Reset()
        {
            MethodCode = new TMethodCode();
            method = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName("void"), string.Empty);
        }
    }
}
