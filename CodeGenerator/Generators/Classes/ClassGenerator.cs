using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using CodeGenerator.Generators.Mappers;
using Domain.AbstractRepositories.Types.Classes;
using Domain.Entities;
using Domain.Entities.Methods;
using Domain.Entities.Types.Classes;

namespace CodeGenerator.Generators.Classes
{
    public class ClassGenerator : ClassGeneratorBase<ClassEntityBase, ClassDeclarationSyntax>, IClassGenerator<ClassEntityBase, MethodEntityBase>
    {
        public IClassGenerator<ClassEntityBase, MethodEntityBase> Initialize(string className, Modifiers modifiers)
        {
            AccessModifiersMapper accessModifiersMapper = new AccessModifiersMapper();
            var syntaxTokens = accessModifiersMapper.From(modifiers);

            @class = SyntaxFactory
                .ClassDeclaration(className)
                .AddModifiers(syntaxTokens);

            return this;
        }

        public IClassGenerator<ClassEntityBase, MethodEntityBase> SetFields()
        {
            //@class.Members.AddRange(new CodeTypeMemberCollection(fields.ToArray<CodeTypeMember>()));
            return this;
        }

        public IClassGenerator<ClassEntityBase, MethodEntityBase> SetProperties()
        {
            //@class.Members.AddRange(new CodeTypeMemberCollection(properties.ToArray<CodeTypeMember>()));
            return this;
        }

        public IClassGenerator<ClassEntityBase, MethodEntityBase> SetMethods(params MethodEntityBase[] methods)
        {
            //@class = @class.AddMembers(methods.Select(m => m.Method).ToArray());
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
