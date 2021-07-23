using System;

using Microsoft.Extensions.DependencyInjection;

using ASL.CodeGenerator.ServiceCollectionExtensions;
using ASL.CodeGenerator.Services;
using ASL.CodeGenerator.StartupClass;

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
                           .AddSingleton<ServicesService>()
                           .AddSingleton<RepositoriesService>()
                           .AddSingleton<Func<ServiceType, IServicesService>>((serviceProvider) =>
                           {
                               return new Func<ServiceType, IServicesService>((serviceType) =>
                               {
                                   switch (serviceType)
                                   {
                                       case ServiceType.Service: return serviceProvider.GetService<ServicesService>();
                                       case ServiceType.Repository: return serviceProvider.GetService<RepositoriesService>();
                                       default: return serviceProvider.GetService<ServicesService>();
                                   }
                               });
                           })
                       ??
                       new ServiceCollection()
                           .AddSingleton<IServiceCollectionExtensionsService, ServiceCollectionExtensionsService>()
                           .AddSingleton<IStartupClassService, StartupClassService>()
                           .AddSingleton<ServicesService>()
                           .AddSingleton<RepositoriesService>()
                           .AddSingleton<Func<ServiceType, IServicesService>>((serviceProvider) =>
                           {
                               return new Func<ServiceType, IServicesService>((serviceType) =>
                               {
                                   switch (serviceType)
                                   {
                                       case ServiceType.Service: return serviceProvider.GetService<ServicesService>();
                                       case ServiceType.Repository: return serviceProvider.GetService<RepositoriesService>();
                                       default: return serviceProvider.GetService<ServicesService>();
                                   }
                               });
                           });
        }
    }
}