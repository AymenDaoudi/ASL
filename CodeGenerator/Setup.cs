using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

using CodeGenerator.Generators.Mappers;
using Domain.AbstractRepositories.Modifiers;

namespace CodeGenerator
{
    public static class Setup
    {
        public static IServiceCollection Services { get; }

        static Setup()
        {
            Services = new ServiceCollection()
                .AddSingleton(typeof(IAccessModifierMapper<SyntaxToken>), typeof(AccessModifiersMapper));
        }
    }
}