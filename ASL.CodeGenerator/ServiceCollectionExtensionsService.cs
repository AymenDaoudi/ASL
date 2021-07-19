using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using ASL.CodeGenerator.Exceptions;
using CSCG.Abstract.Generators.Types.Classes;
using CSCG.Abstract.Generators.Statements;
using CSCG.Abstract.Generators.Namespaces;
using CSCG.Abstract.Generators.Methods;
using CSCG.Abstract.Generators.Files;
using CSCG.Abstract.Generators.Expressions;
using CSCG.Abstract.Entities.Expressions;
using CSCG.Abstract.Entities.Files;
using CSCG.Abstract.Entities.Methods;
using CSCG.Abstract.Entities.Methods.Classes;
using CSCG.Abstract.Entities.Namespaces;
using CSCG.Abstract.Entities.Statements;
using CSCG.Abstract.Entities.Types.Classes;
using CSCG.Abstract.Entities.Types;
using CSCG.Abstract.Entities;
using CSCG.Abstract.Repositories;
using CSCG.Roslyn.Exceptions;
using static ASL.CodeGenerator.Consts;

namespace ASL.CodeGenerator
{
    public class ServiceCollectionExtensionsService : IServiceCollectionExtensionsService
    {
        private const string ISERVICE_COLLECTION_EXTENSIONS = "IServiceCollectionExtensions";
        private const string ADD_SCOPED = "AddScoped";
        private const string ADD_TRANSIENT = "AddTransient";
        private const string ADD_SINGLETON = "AddSingleton";

        private readonly ICodeFileModifier _classModifier;
        private readonly IClassGenerator<ClassEntityBase, ClassMethodEntity> _classGenerator;
        private readonly INamespaceGenerator<NamespaceEntityBase<TypeEntityBase>, TypeEntityBase> _namespaceGenerator;
        private readonly ICodeFileGenerator<TypeEntityBase> _codeFileGenerator;
        private readonly IMethodRepository _methodRepository;
        private readonly ICodeFileReader<CodeFileEntityBase> _codeFileReader;
        private readonly IStatementGenerator<ReturnStatementEntity, ExpressionEntityBase> _returnStatementGenerator;
        private readonly IExtensionMethodGenerator<ExtensionMethodEntity, StatementEntityBase, ParameterEntityBase> _extensionMethodGenerator;
        private readonly IObjectExpressionGenerator _objectExpressionGenerator;
        private readonly IMethodInvocationExpressionGenerator _methodInvocationExpressionGenerator;

        public ServiceCollectionExtensionsService(
            ICodeFileModifier classModifier,
            IClassGenerator<ClassEntityBase, ClassMethodEntity> classGenerator,
            INamespaceGenerator<NamespaceEntityBase<TypeEntityBase>, TypeEntityBase> namespaceGenerator,
            ICodeFileGenerator<TypeEntityBase> codeFileGenerator,
            IMethodRepository methodRepository,
            ICodeFileReader<CodeFileEntityBase> codeFileReader,
            IStatementGenerator<ReturnStatementEntity, ExpressionEntityBase> returnStatementGenerator,
            IExtensionMethodGenerator<ExtensionMethodEntity, StatementEntityBase, ParameterEntityBase> extensionMethodGenerator,
            IObjectExpressionGenerator objectExpressionGenerator,
            IMethodInvocationExpressionGenerator methodInvocationExpression
        )
        {
            _classModifier = classModifier;
            _classGenerator = classGenerator;
            _namespaceGenerator = namespaceGenerator;
            _codeFileGenerator = codeFileGenerator;
            _methodRepository = methodRepository;
            _codeFileReader = codeFileReader;
            _returnStatementGenerator = returnStatementGenerator;
            _extensionMethodGenerator = extensionMethodGenerator;
            _objectExpressionGenerator = objectExpressionGenerator;
            _methodInvocationExpressionGenerator = methodInvocationExpression;
        }

        public void CreateFile(
            string filePath,
            string namespaceName,
            params string[] usings
        )
        {
            var modifiers = AccessModifiers.Public | AccessModifiers.Static;

            var @class = _classGenerator
                .Initialize(className: ISERVICE_COLLECTION_EXTENSIONS, modifiers)
                .SetMethods(GenerateMethods().ToArray())
                .Generate();

            var @namespace = _namespaceGenerator.Initialize(namespaceName)
                .SetMemebers(@class)
                .Generate();

            _codeFileGenerator.CreateFile(filePath, @namespace, usings);
        }

        public Task RegisterNewRepositoryAsync(
            string filePath,
            DILifetime dILifeTime,
            string abstractTypeName,
            string ImplementationTypeName
        )
        {
            return AddDiMethodToReturnStatementAsync(
                filePath,
                dILifeTime,
                REGISTER_REPOSITORIES,
                abstractTypeName,
                ImplementationTypeName
            );
        }

        public Task RegisterNewRepositoryAsync(
            string filePath,
            DILifetime dILifeTime,
            string ImplementationTypeName
        )
        {
            return AddDiMethodToReturnStatementAsync(
                filePath,
                dILifeTime,
                REGISTER_REPOSITORIES,
                ImplementationTypeName
            );
        }

        public Task RegisterNewServiceAsync(
            string filePath,
            DILifetime dILifeTime,
            string abstractTypeName,
            string ImplementationTypeName
        )
        {
            return AddDiMethodToReturnStatementAsync(
                filePath,
                dILifeTime,
                REGISTER_SERVICES,
                abstractTypeName,
                ImplementationTypeName
            );
        }

        public Task RegisterNewServiceAsync(
            string filePath,
            DILifetime dILifeTime,
            string ImplementationTypeName
        )
        {

            return AddDiMethodToReturnStatementAsync(
                filePath,
                dILifeTime,
                REGISTER_SERVICES,
                ImplementationTypeName
            );
        }

        private async Task AddDiMethodToReturnStatementAsync(
            string filePath,
            DILifetime dILifeTime,
            string methodName,
            params string[] types
        )
        {
            var @class = await _codeFileReader.ReadClassAsync(filePath, ISERVICE_COLLECTION_EXTENSIONS);

            var lifeTimeDiMethodName = dILifeTime switch
            {
                DILifetime.Scoped => ADD_SCOPED,
                DILifetime.Transient => ADD_TRANSIENT,
                DILifetime.Singleton => ADD_SINGLETON,
                _ => ADD_SCOPED,
            };

            StatementEntityBase returnStatement;

            try
            {
                returnStatement = _methodRepository.GetReturnStatement(@class, methodName);
            }
            catch (NoOrMultipleMethodsException exception) when (exception.Message == string.Format(NoOrMultipleMethodsException.NO_METHOD_ERROR_MESSAGE, methodName))
            {
                throw new NoOrMultipleServiceCollectionExtensionsMethodsException(string.Format(NoOrMultipleServiceCollectionExtensionsMethodsException.NO_METHOD_ERROR_MESSAGE, methodName), exception);
            }
            catch (NoOrMultipleMethodsException exception) when (exception.Message == string.Format(NoOrMultipleMethodsException.MULTIPLE_METHODS_ERROR_MESSAGE, methodName))
            {
                throw new NoOrMultipleServiceCollectionExtensionsMethodsException(string.Format(NoOrMultipleServiceCollectionExtensionsMethodsException.MULTIPLE_METHODS_ERROR_MESSAGE, methodName), exception);
            }

            var methodInvocationExpression = _methodInvocationExpressionGenerator.Initialize(
                returnStatement.Expression,
                lifeTimeDiMethodName,
                types
            ).Generate();

            var newRturnStatement = _returnStatementGenerator.Generate(methodInvocationExpression);

            await _classModifier.ReplaceReturnStatementOfMethodOfClassAsync(
                filePath,
                ISERVICE_COLLECTION_EXTENSIONS,
                methodName,
                newRturnStatement
            );
        }

        private IEnumerable<ExtensionMethodEntity> GenerateMethods()
        {
            var objectExpression = _objectExpressionGenerator
                .Initialize(SERVICES)
                .Generate();

            var returnStatement = _returnStatementGenerator.Generate(objectExpression);

            var registerRepositoriesMethod = _extensionMethodGenerator
                .Initialize(
                    methodName: REGISTER_REPOSITORIES,
                    returnTypeName: nameof(IServiceCollection),
                    nameof(IServiceCollection),
                    SERVICES)
                .SetStatements(returnStatement)
                .Generate();

            var registerServicesMethod = _extensionMethodGenerator
                .Initialize(REGISTER_SERVICES,
                    returnTypeName: nameof(IServiceCollection),
                    nameof(IServiceCollection),
                    SERVICES)
                .SetStatements(returnStatement)
                .Generate();

            return new ExtensionMethodEntity[] { registerRepositoriesMethod, registerServicesMethod };
        }
    }
}