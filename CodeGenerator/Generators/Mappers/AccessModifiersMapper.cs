using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using CodeGenerator.Abstract.Generators.Modifiers;
using CodeGenerator.Abstract.Entities;

namespace CodeGenerator.Roslyn.Generators.Mappers
{
    internal class AccessModifiersMapper : IAccessModifierMapper<SyntaxToken>
    {
        public SyntaxToken[] From(AccessModifiers accessModifier)
        {
            var synatxTokens = accessModifier switch
            {
                AccessModifiers.None => new SyntaxToken[] { },
                AccessModifiers.Private => new SyntaxToken[] { SyntaxFactory.Token(SyntaxKind.PrivateKeyword) },
                AccessModifiers.Public => new SyntaxToken[] { SyntaxFactory.Token(SyntaxKind.PublicKeyword) },
                AccessModifiers.Protected => new SyntaxToken[] { SyntaxFactory.Token(SyntaxKind.ProtectedKeyword) },
                AccessModifiers.Static => new SyntaxToken[] { SyntaxFactory.Token(SyntaxKind.StaticKeyword) },
                (AccessModifiers.Static | AccessModifiers.Private) => new SyntaxToken[]
                {
                    SyntaxFactory.Token(SyntaxKind.StaticKeyword),
                    SyntaxFactory.Token(SyntaxKind.PrivateKeyword)
                },
                (AccessModifiers.Static | AccessModifiers.Public) => new SyntaxToken[]
                {
                    SyntaxFactory.Token(SyntaxKind.StaticKeyword),
                    SyntaxFactory.Token(SyntaxKind.PublicKeyword)
                },
                (AccessModifiers.Static | AccessModifiers.Protected) => new SyntaxToken[]
                {
                    SyntaxFactory.Token(SyntaxKind.StaticKeyword),
                    SyntaxFactory.Token(SyntaxKind.ProtectedKeyword)
                },
                _ => new SyntaxToken[] { }
            };

            return synatxTokens;
        }

        public AccessModifiers To(SyntaxToken[] syntaxTokens)
        {
            var syntaxKinds = syntaxTokens
                .Select(t => t.Kind())
                .ToArray();

            AccessModifiers modifiers = To(syntaxKinds.First());

            var syntaxKindsCounts = syntaxKinds.Count();

            if (syntaxKindsCounts > 1)
            {
                return modifiers;
            }

            for (int i = 1; i < syntaxKindsCounts; i++)
            {
                modifiers |= To(syntaxKinds[i]);
            }

            return modifiers;
        }

        private AccessModifiers To(SyntaxKind syntaxKind)
        {
            var modifier = syntaxKind switch
            {
                SyntaxKind.None => AccessModifiers.None,
                SyntaxKind.PrivateKeyword => AccessModifiers.Private,
                SyntaxKind.PublicKeyword => AccessModifiers.Public,
                SyntaxKind.ProtectedKeyword => AccessModifiers.Protected,
                SyntaxKind.StaticKeyword => AccessModifiers.Static,
                _ => AccessModifiers.None
            };

            return modifier;
        }
    }
}