﻿using System.IO;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Domain.Entities.Namespaces;
using Domain.Entities.Types;

namespace CodeGenerator.Generators
{
    public class CodeFileGeneratorBase<TType> : ICodeFileGenerator<TType> where TType : TypeEntityBase
    {
        public CodeFileGeneratorBase()
        {

        }

        public void CreateFile(
            string filePath,
            NamespaceEntityBase<TType> namespaceEntity,
            params string[] usings
        )
        {
            var compilationUnit = SyntaxFactory.CompilationUnit(
                new SyntaxList<ExternAliasDirectiveSyntax>(),
                new SyntaxList<UsingDirectiveSyntax>(usings.Select(@using => SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(@using)))),
                new SyntaxList<AttributeListSyntax>(),
                new SyntaxList<MemberDeclarationSyntax>((NamespaceDeclarationSyntax)namespaceEntity.NamespaceRoot));

            using (FileStream fs = File.Create(filePath))
            {
                using (StreamWriter sourceWriter = new StreamWriter(fs))
                {
                    compilationUnit.NormalizeWhitespace().WriteTo(sourceWriter);
                }
            }
        }
    }
}