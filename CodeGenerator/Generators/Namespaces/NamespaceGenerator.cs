using System.Linq;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Domain.AbstractRepositories.Namespaces;
using Domain.Entities.Namespaces;
using Domain.Entities.Types;

namespace CodeGenerator.Generators.Namespaces
{
    public class NamespaceGenerator : NamespaceGeneratorBase<NamespaceEntityBase<TypeEntityBase>, NamespaceDeclarationSyntax>, 
        INamespaceGenerator<NamespaceEntityBase<TypeEntityBase>, TypeEntityBase>
    {
        public virtual INamespaceGenerator<NamespaceEntityBase<TypeEntityBase>, TypeEntityBase> Initialize(string namespaceName)
        {
            @namespace = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(namespaceName));

            return this;
        }

        public INamespaceGenerator<NamespaceEntityBase<TypeEntityBase>, TypeEntityBase> SetMemebers(params TypeEntityBase[] members)
        {
            @namespace = @namespace.AddMembers(members.Select(m => (MemberDeclarationSyntax)m.TypeRoot).ToArray());

            return this;
        }

        protected override NamespaceEntityBase<TypeEntityBase> GenerateNamespaceEntity()
        {
            var namespaceEntity = new NamespaceEntityBase<TypeEntityBase>(
                @namespace.Name.ToString(),
                @namespace,
                @namespace.Members.Select(m => new TypeEntityBase(((TypeDeclarationSyntax)m).Identifier.ValueText, (TypeDeclarationSyntax)m))
            );

            return namespaceEntity;
        }
    }
}