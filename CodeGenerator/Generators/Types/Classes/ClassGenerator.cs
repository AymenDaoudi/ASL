using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CodeGenerator.Abstract.Generators.Types.Classes;
using CodeGenerator.Abstract.Generators.Modifiers;
using CodeGenerator.Abstract.Entities.Methods;
using CodeGenerator.Abstract.Entities.Types.Classes;
using CodeGenerator.Abstract.Entities;

namespace CodeGenerator.Roslyn.Generators.Types.Classes
{
    public class ClassGenerator : IClassGenerator<ClassEntityBase, MethodEntityBase>
    {
        private readonly IAccessModifierMapper<SyntaxToken> _accessModifierMapper;

        public ClassGenerator(IAccessModifierMapper<SyntaxToken> accessModifierMapper)
        {
            _accessModifierMapper = accessModifierMapper;
        }

        public IInitializedClassGenerator<ClassEntityBase, MethodEntityBase> Initialize(string className, AccessModifiers modifiers)
        {
            var syntaxTokens = _accessModifierMapper.From(modifiers);

            var @class = SyntaxFactory
                .ClassDeclaration(className)
                .AddModifiers(syntaxTokens);

            var initializedClassGenerator = new InitializedClassGenerator(@class);

            return initializedClassGenerator;
        }

        private class InitializedClassGenerator : ClassGeneratorBase<ClassEntityBase, ClassDeclarationSyntax>, IInitializedClassGenerator<ClassEntityBase, MethodEntityBase>
        {
            public InitializedClassGenerator(ClassDeclarationSyntax @class)
            {
                this.@class = @class;
            }

            public IInitializedClassGenerator<ClassEntityBase, MethodEntityBase> SetFields()
            {
                //@class.Members.AddRange(new CodeTypeMemberCollection(fields.ToArray<CodeTypeMember>()));
                return this;
            }

            public IInitializedClassGenerator<ClassEntityBase, MethodEntityBase> SetProperties()
            {
                //@class.Members.AddRange(new CodeTypeMemberCollection(properties.ToArray<CodeTypeMember>()));
                return this;
            }

            public IInitializedClassGenerator<ClassEntityBase, MethodEntityBase> SetMethods(params MethodEntityBase[] methods)
            {
                @class = @class.AddMembers(methods.Select(m => (MemberDeclarationSyntax)m.Method).ToArray());
                return this;
            }

            protected override ClassEntityBase GenerateClassEntity(ClassDeclarationSyntax classRoot)
            {
                var classEntity = new ClassEntityBase(
                    classRoot.Identifier.ValueText,
                    classRoot
                );

                return classEntity;
            }
        }
    }
}