using System.Linq;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using CodeGenerator.Generators.Mappers;
using Domain.AbstractRepositories.Types.Classes;
using Domain.Entities;
using Domain.Entities.Methods;
using Domain.Entities.Types.Classes;

namespace CodeGenerator.Generators.Classes
{
    public class ClassGenerator : IClassGenerator<ClassEntityBase, MethodEntityBase>
    {
        public IInitializedClassGenerator<ClassEntityBase, MethodEntityBase> Initialize(string className, Modifiers modifiers)
        {
            AccessModifiersMapper accessModifiersMapper = new AccessModifiersMapper();
            var syntaxTokens = accessModifiersMapper.From(modifiers);

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