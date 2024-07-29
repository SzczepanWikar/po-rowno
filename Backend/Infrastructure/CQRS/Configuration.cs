using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.CQRS
{
    public static class Configuration
    {
        public static IServiceCollection AddCQRS(this IServiceCollection services)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblies));

            return services;
        }

        public static WebApplication UseCQRS(this WebApplication app) => app;
    }
}
