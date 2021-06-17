using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace CodeGenerator
{
    public class IServiceCollectionExtenderCodeGenerator
    {
        public IServiceCollectionExtenderCodeGenerator()
        {
            var codeCompileUnit = GenerateCode();
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BracingStyle = "C";
            var outputFileName = @"C:\Users\aymen.daoudi\Desktop\IServiceCollectionExtensions.cs";

            using (FileStream fs = File.Create(outputFileName))
            {
                using (StreamWriter sourceWriter = new StreamWriter(fs))
                {
                    using (var stringWriter = new StringWriter())
                    {
                        provider.GenerateCodeFromCompileUnit(
                            codeCompileUnit,
                            stringWriter,
                            options
                        );

                        var changed = stringWriter.ToString().Replace("public class", "public static class");

                        sourceWriter.Write(changed);
                    }
                }
            }
        }

        public CodeCompileUnit GenerateCode()
        {
            var targetUnit = new CodeCompileUnit();

            var codeNamspace= new CodeNamespace("Miscellaneous");
            codeNamspace.Imports.Add(new CodeNamespaceImport("System"));
            codeNamspace.Imports.Add(new CodeNamespaceImport("Microsoft.Extensions.DependencyInjection"));
            
            var targetClass = new CodeTypeDeclaration("IServiceCollectionExtensions");
            targetClass.IsClass = true;
            targetClass.TypeAttributes = TypeAttributes.Public;
            
            var servicesRegistrationMethod = CreateServicesRegistrationMethod("RegisterRepositories");
            var repositoriesRegistrationMethod = CreateServicesRegistrationMethod("RegisterServices");
            
            targetClass.Members.Add(servicesRegistrationMethod);
            targetClass.Members.Add(repositoriesRegistrationMethod);
            
            codeNamspace.Types.Add(targetClass);
            
            targetUnit.Namespaces.Add(codeNamspace);

            return targetUnit;
        }

        private CodeMemberMethod CreateServicesRegistrationMethod(string methodName)
        {
            CodeMemberMethod registerRepositoriesMethod = new CodeMemberMethod();
            
            registerRepositoriesMethod.Attributes = MemberAttributes.Public | MemberAttributes.Static;
            registerRepositoriesMethod.Name = methodName;
            
            var servicesParam = new CodeParameterDeclarationExpression($"this {nameof(IServiceCollection)}", "services");
            
            registerRepositoriesMethod.Parameters.Add(servicesParam);
            registerRepositoriesMethod.ReturnType =
                new CodeTypeReference(nameof(IServiceCollection), CodeTypeReferenceOptions.GenericTypeParameter);

            var returnStatement = new CodeMethodReturnStatement();
            returnStatement.Expression = new CodeArgumentReferenceExpression("services");

            registerRepositoriesMethod.Statements.Add(returnStatement);

            return registerRepositoriesMethod;
        }
    }
}
