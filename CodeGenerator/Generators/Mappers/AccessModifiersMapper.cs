using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using Domain.Entities;

namespace CodeGenerator.Generators.Mappers
{
    public class AccessModifiersMapper
    {
        public SyntaxToken[] From(Modifiers modifier)
        {
            var synatxTokens = modifier switch
            {
                Modifiers.None => new SyntaxToken[] { },
                Modifiers.Private => new SyntaxToken[] { SyntaxFactory.Token(SyntaxKind.PrivateKeyword) },
                Modifiers.Public => new SyntaxToken[] { SyntaxFactory.Token(SyntaxKind.PublicKeyword) },
                Modifiers.Protected => new SyntaxToken[] { SyntaxFactory.Token(SyntaxKind.ProtectedKeyword) },
                Modifiers.Static => new SyntaxToken[] { SyntaxFactory.Token(SyntaxKind.StaticKeyword) },
                (Modifiers.Static | Modifiers.Private) => new SyntaxToken[]
                {
                    SyntaxFactory.Token(SyntaxKind.StaticKeyword),
                    SyntaxFactory.Token(SyntaxKind.PrivateKeyword)
                },
                (Modifiers.Static | Modifiers.Public) => new SyntaxToken[]
                {
                        SyntaxFactory.Token(SyntaxKind.StaticKeyword),
                        SyntaxFactory.Token(SyntaxKind.PublicKeyword)
                },
                (Modifiers.Static | Modifiers.Protected) => new SyntaxToken[]
                {
                        SyntaxFactory.Token(SyntaxKind.StaticKeyword),
                        SyntaxFactory.Token(SyntaxKind.ProtectedKeyword)
                },
                _ => new SyntaxToken[] { }
            };

            return synatxTokens;
        }

        public Modifiers To(SyntaxToken[] syntaxTokens)
        {
            var syntaxKinds = syntaxTokens
                .Select(t => t.Kind())
                .ToArray();

            Modifiers modifiers = To(syntaxKinds.First());

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

        private Modifiers To(SyntaxKind syntaxKind)
        {
            var modifier = syntaxKind switch
            {
                SyntaxKind.None => Modifiers.None,
                SyntaxKind.PrivateKeyword => Modifiers.Private,
                SyntaxKind.PublicKeyword => Modifiers.Public,
                SyntaxKind.ProtectedKeyword => Modifiers.Protected,
                SyntaxKind.StaticKeyword => Modifiers.Static,
                _ => Modifiers.None
            };

            return modifier;
        }
    }
}
