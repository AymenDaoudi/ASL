using Microsoft.Extensions.DependencyInjection;

namespace ASL.CodeGenerator
{
    public class Startup
    {
        public static IServiceCollection Services { get; }

        static Startup()
        {
            Services = CSCG.Roslyn.Setup.Services?
                    .AddSingleton<IServiceCollectionExtensionsService, ServiceCollectionExtensionsService>()
                    .AddSingleton<IStartupClassService, StartupClassService>()
                    .AddSingleton<IServicesService, ServicesService>() ?? 
                new ServiceCollection()
                    .AddSingleton<IServiceCollectionExtensionsService, ServiceCollectionExtensionsService>()
                    .AddSingleton<IStartupClassService, StartupClassService>()
                    .AddSingleton<IServicesService, ServicesService>();
        }
    }
}