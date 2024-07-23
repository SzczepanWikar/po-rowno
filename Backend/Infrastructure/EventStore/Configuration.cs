using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.EventStore
{
    public static class Configuration
    {
        public static IServiceCollection AddEventStore(
            this IServiceCollection services,
            IConfiguration config
        )
        {
            services.AddEventStoreClient(config.GetConnectionString("EventStore"));
            services.AddEventStorePersistentSubscriptionsClient(
                config.GetConnectionString("EventStore")
            );
            services.AddSingleton(EventParser.Instance);
            return services;
        }

        public static WebApplication UseEventStore(this WebApplication app) => app;
    }
}
