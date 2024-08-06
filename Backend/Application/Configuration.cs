using Application.User.Auth;
using Infrastructure.EventStore.Repository;
using Infrastructure.Projections;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class Configuration
    {
        public static IServiceCollection AddBusinessLogic(
            this IServiceCollection services,
            IConfiguration config
        )
        {
            services.AddScoped<EmailConflictValidator>();
            services.AddEventStoreRepository<Core.Foo.Foo>();
            services.AddEventStoreRepository<Core.User.User>();
            services.AddProjections<ApplicationContext>(config);

            return services;
        }

        public static WebApplication UseBusinessLogic(this WebApplication app) => app;
    }
}
