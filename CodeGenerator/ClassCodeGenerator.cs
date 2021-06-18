using System.CodeDom;
using System.Linq;
using System.Reflection;

namespace CodeGenerator
{
    public class ClassCodeGenerator : CodeFileAbstractGenerator
    {
        public ClassCodeGenerator(
            string @namespace,
            string @class,
            TypeAttributes modifier,
            bool isStatic,
            params string[] usings
        ): base(
            @namespace,
            @class,
            modifier,
            isStatic,
            usings
        )
        {
        }

        protected override CodeCompileUnit GenerateCode()
        {
            var targetUnit = new CodeCompileUnit();

            var codeNamspace = new CodeNamespace(@namespace);
            usings.ToList().ForEach(@using => codeNamspace.Imports.Add(new CodeNamespaceImport(@using)));
            
            var targetClass = new CodeTypeDeclaration(@class);
            targetClass.IsClass = true;
            targetClass.TypeAttributes = modifier;
            if (isStatic)
            {
                codeSubsitutions.Add($"public class {@class}", $"public static class {@class}");
            }

            methods.ToList().ForEach(method => targetClass.Members.Add(method));

            codeNamspace.Types.Add(targetClass);

            targetUnit.Namespaces.Add(codeNamspace);

            return targetUnit;
        }
    }
}
