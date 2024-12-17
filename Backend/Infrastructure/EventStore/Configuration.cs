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
            var connectionString = config.GetConnectionString("EventStore");

            if (connectionString == null)
            {
                throw new InvalidOperationException("EventStoreDB connection string is missing.");
            }

            services.AddEventStoreClient(connectionString);
            services.AddEventStorePersistentSubscriptionsClient(connectionString);
            services.AddEventStoreProjectionManagementClient(connectionString);

            services.AddSingleton<EventParser>();

            return services;
        }

        public static WebApplication UseEventStore(this WebApplication app) => app;
    }
}
