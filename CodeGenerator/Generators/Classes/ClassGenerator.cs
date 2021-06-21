using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeGenerator.Generators.Classes
{
    public class ClassGenerator : ClassAbstractGenerator
    {
        public override ClassGenerator SetFields()
        {
            //@class.Members.AddRange(new CodeTypeMemberCollection(fields.ToArray<CodeTypeMember>()));
            return this;
        }

        public override ClassGenerator SetMethods(params MethodDeclarationSyntax[] methods)
        {
            @class = @class.AddMembers(methods);
            return this;
        }

        public override ClassGenerator SetProperties()
        {
            //@class.Members.AddRange(new CodeTypeMemberCollection(properties.ToArray<CodeTypeMember>()));
            return this;
        }
    }
}
