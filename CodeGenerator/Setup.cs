using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

using CodeGenerator.Roslyn.Generators.Expressions;
using CodeGenerator.Roslyn.Generators.Mappers;
using CodeGenerator.Roslyn.Generators.Methods;
using CodeGenerator.Roslyn.Generators.Namespaces;
using CodeGenerator.Roslyn.Generators.Statements;
using CodeGenerator.Roslyn.Generators.Types.Classes;
using CodeGenerator.Roslyn.Generators.Files;
using CodeGenerator.Abstract.Generators.Types.Classes;
using CodeGenerator.Abstract.Generators.Statements;
using CodeGenerator.Abstract.Generators.Namespaces;
using CodeGenerator.Abstract.Generators.Modifiers;
using CodeGenerator.Abstract.Generators.Methods;
using CodeGenerator.Abstract.Generators.Files;
using CodeGenerator.Abstract.Generators.Expressions;
using CodeGenerator.Abstract.Entities.Expressions;
using CodeGenerator.Abstract.Entities.Files;
using CodeGenerator.Abstract.Entities.Methods;
using CodeGenerator.Abstract.Entities.Namespaces;
using CodeGenerator.Abstract.Entities.Statements;
using CodeGenerator.Abstract.Entities.Types.Classes;
using CodeGenerator.Abstract.Entities.Types;
using CodeGenerator.Abstract.Repositories;
using CodeGenerator.Roslyn.Repositories;

namespace CodeGenerator.Roslyn
{
    public static class Setup
    {
        public static IServiceCollection Services { get; }

        static Setup()
        {
            Services = new ServiceCollection()
                .AddSingleton(typeof(ICodeFileReader<CodeFileEntityBase>), typeof(CodeFileReader))
                .AddSingleton(typeof(ICodeFileGenerator<TypeEntityBase>), typeof(CodeFileGenerator))
                .AddSingleton<ICodeFileModifier, CodeFileModifier>()
                .AddSingleton(typeof(INamespaceGenerator<NamespaceEntityBase<TypeEntityBase>, TypeEntityBase>), typeof(NamespaceGenerator))
                .AddSingleton(typeof(IClassGenerator<ClassEntityBase, MethodEntityBase>), typeof(ClassGenerator))
                .AddSingleton<IMethodRepository, MethodRepository>()
                .AddSingleton<IExpressionRepository, ExpressionRepository>()
                .AddSingleton(typeof(IExtensionMethodGenerator<ExtensionMethodEntity, StatementEntityBase, ParameterEntityBase>), typeof(ExtensionMethodGenerator))
                .AddSingleton(typeof(IInstanceMethodGenerator<MethodEntityBase, StatementEntityBase, ParameterEntityBase>), typeof(MethodGenerator))
                .AddSingleton(typeof(IStatementGenerator<StatementEntityBase, ExpressionEntityBase>), typeof(StatementGenerator))
                .AddSingleton<IMethodInvocationExpressionGenerator, MethodInvocationExpressionGenerator>()
                .AddSingleton<IObjectExpressionGenerator, ObjectExpressionGenerator>()
                .AddSingleton(typeof(IAccessModifierMapper<SyntaxToken>), typeof(AccessModifiersMapper));
        }
    }
}