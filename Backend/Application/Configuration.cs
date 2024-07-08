using Microsoft.Extensions.DependencyInjection;
using Infrastructure.EventStore.Repository;

namespace Application
{
    public static class Configuration
    {
        public static IServiceCollection AddBusinessLogic(this IServiceCollection services)
        {
            services.AddEventStoreRepository<Core.Foo.Foo>();
            return services;
        }
    }
}
