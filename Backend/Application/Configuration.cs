using Microsoft.Extensions.DependencyInjection;
using Infrastructure.EventStore.Repository;
using Infrastructure.Projections;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;

namespace Application
{
    public static class Configuration
    {
        public static IServiceCollection AddBusinessLogic(this IServiceCollection services, IConfiguration config)
        {
            services.AddEventStoreRepository<Core.Foo.Foo>();
            services.AddProjections<ApplicationContext>(config);

            return services;
        }

        public static WebApplication UseBusinessLogic(this WebApplication app) => app;
    }
}
