using Application.Group;
using Application.User;
using Core.Common.Projections;
using Core.User.UserToken;
using EventStore.Client;
using Infrastructure.EventStore.Repository;
using Infrastructure.Projections;
using Infrastructure.Projections.InternalProjections.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Application
{
    public static class Configuration
    {
        public static IServiceCollection AddBusinessLogic(
            this IServiceCollection services,
            IConfiguration config
        )
        {
            services.AddEventStoreRepository<Core.Foo.Foo>();
            services.AddEventStoreRepository<Core.User.User>();
            services.AddEventStoreRepository<Core.Group.Group>();
            services.AddEventStoreRepository<UserToken>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IGroupService, GroupService>();

            services.AddKeyedScoped<IIndexProjectionRepository, GroupIndexRepository>("GroupCode");
            services.AddKeyedScoped<IIndexProjectionRepository, UserEmailIndexRepository>(
                "UserEmail"
            );

            services.AddProjections<ApplicationContext>(config);

            return services;
        }

        public static WebApplication UseBusinessLogic(this WebApplication app) => app;
    }
}
