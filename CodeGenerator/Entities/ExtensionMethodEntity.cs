using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeGenerator.Entities
{
    public class ExtensionMethodEntity : MethodEntityBase
    {
        public string ExtendedTypeName { get; }
        public string ExtendedTypeParameterName { get; }

        public ExtensionMethodEntity(
            string methodName, 
            string extendedTypeName, 
            string extendedTypeParameterName
        )
        : base(methodName, new SyntaxToken[]
          {
              SyntaxFactory.Token(SyntaxKind.PublicKeyword),
              SyntaxFactory.Token(SyntaxKind.StaticKeyword)
          })
        {
            ExtendedTypeName = extendedTypeName;
            ExtendedTypeParameterName = extendedTypeParameterName;
        }

        public ExtensionMethodEntity()
        : base()
        {
        }
    }
}