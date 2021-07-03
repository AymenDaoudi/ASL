using System.Linq;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Domain.AbstractRepositories.Namespaces;
using Domain.Entities.Namespaces;
using Domain.Entities.Types;

namespace CodeGenerator.Generators.Namespaces
{
    public class NamespaceGenerator : INamespaceGenerator<NamespaceEntityBase<TypeEntityBase>, TypeEntityBase>
    {
        public IInitializedNamespaceGenerator<NamespaceEntityBase<TypeEntityBase>, TypeEntityBase> Initialize(string namespaceName)
        {
            var @namespace = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(namespaceName));

            var initializedNamespaceGenerator = new InitializedNamespaceGenerator(@namespace);

            return initializedNamespaceGenerator;
        }

        private class InitializedNamespaceGenerator : NamespaceGeneratorBase<NamespaceEntityBase<TypeEntityBase>, NamespaceDeclarationSyntax>, IInitializedNamespaceGenerator<NamespaceEntityBase<TypeEntityBase>, TypeEntityBase>
        {
            public InitializedNamespaceGenerator(NamespaceDeclarationSyntax @namespace)
            {
                this.@namespace = @namespace;
            }

            public IInitializedNamespaceGenerator<NamespaceEntityBase<TypeEntityBase>, TypeEntityBase> SetMemebers(params TypeEntityBase[] members)
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
}