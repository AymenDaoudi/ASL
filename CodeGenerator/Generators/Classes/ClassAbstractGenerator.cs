using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeGenerator.Generators.Classes
{
    public abstract class ClassAbstractGenerator
    {
        protected ClassDeclarationSyntax @class;

        public virtual ClassAbstractGenerator Initialize(string className, SyntaxToken[] modifiers)
        {
            @class = SyntaxFactory
                .ClassDeclaration(className)
                .AddModifiers(modifiers);

            return this;
        }

        public abstract ClassAbstractGenerator SetFields();

        public abstract ClassAbstractGenerator SetMethods(params MethodDeclarationSyntax[] methods);

        public abstract ClassAbstractGenerator SetProperties();

        public virtual ClassDeclarationSyntax Generate()
        {
            var generatedClass = SyntaxFactory.ClassDeclaration(string.Empty);
            generatedClass = @class;

            Reset();

            return generatedClass;
        }

        public void Reset()
        {
            @class = SyntaxFactory.ClassDeclaration(string.Empty);
        }
    }
}
