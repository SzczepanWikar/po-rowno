using Microsoft.Extensions.DependencyInjection;
using Infrastructure.EventStore.Repository;
using Infrastructure.Projections;
using Microsoft.Extensions.Configuration;

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
    }
}
