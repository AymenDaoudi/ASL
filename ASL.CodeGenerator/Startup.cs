using Microsoft.Extensions.DependencyInjection;

using CSCG.Roslyn;

namespace Services
{
    public class Startup
    {
        public static IServiceCollection Services { get; }

        static Startup()
        {
            Services = (Setup.Services ?? new ServiceCollection());
        }
    }
}