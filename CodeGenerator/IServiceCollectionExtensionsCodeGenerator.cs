using System.CodeDom;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace CodeGenerator
{
    public class IServiceCollectionExtensionsCodeGenerator : ClassCodeGenerator
    {
        public IServiceCollectionExtensionsCodeGenerator(
            string @namespace,
            string @class,
            TypeAttributes modifier,
            bool isStatic,
            params string[] usings
        ) : base(
            @namespace,
            @class,
            modifier,
            isStatic,
            usings
        )
        {
            var servicesRegistrationMethod = CreateServicesRegistrationMethod("RegisterRepositories");
            var repositoriesRegistrationMethod = CreateServicesRegistrationMethod("RegisterServices");

            methods.Add(servicesRegistrationMethod);
            methods.Add(repositoriesRegistrationMethod);
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
