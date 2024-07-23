using Core.Common.Aggregate;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.EventStore.Repository
{
    public static class Configuration
    {
        public static IServiceCollection AddEventStoreRepository<T>(
            this IServiceCollection services
        )
            where T : class, IAggregate =>
            services.AddScoped<IEventStoreRepository<T>, BaseEventStoreRepository<T>>();
    }
}
