using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeGenerator.Entities
{
    public class MethodEntityBase
    {
        public string MethodName { get; }
        public SyntaxToken[] Modifiers { get; }
        public TypeSyntax ReturnType { get; set; }
        public ICollection<ParameterSyntax> Parameters { get; set; }
        public ICollection<StatementSyntax> CodeStatements { get; set; }

        public MethodEntityBase(string methodName, SyntaxToken[] modifiers)
        {
            MethodName = methodName;
            Modifiers = modifiers;
            ReturnType = null;
            Parameters = new List<ParameterSyntax>();
            CodeStatements = new List<StatementSyntax>();
        }

        public MethodEntityBase()
        {
        }
    }
}
