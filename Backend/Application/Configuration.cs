using Application.Expense;
using Application.Group;
using Application.User.Repositories;
using Application.User.Services;
using Core.Common.Projections;
using Core.User;
using Infrastructure.EventStore.Repository;
using Infrastructure.PayPal;
using Infrastructure.Projections;
using Infrastructure.Projections.InternalProjections.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
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
            AddRepositories(services);
            AddServices(services);
            AddInfrastructure(services, config);

            return services;
        }

        public static WebApplication UseBusinessLogic(this WebApplication app) => app;

        private static void AddRepositories(IServiceCollection services)
        {
            AddAggregateRepositories(services);
            AddIndexRepositories(services);
        }

        private static void AddIndexRepositories(IServiceCollection services)
        {
            services.AddKeyedScoped<IIndexProjectionRepository, GroupIndexRepository>(
                InternalProjectionName.GroupCodeIndex
            );
            services.AddKeyedScoped<IIndexProjectionRepository, UserEmailIndexRepository>(
                InternalProjectionName.EmailIndex
            );
            services.AddKeyedScoped<IIndexProjectionRepository, PaymentIndexRepository>(
                InternalProjectionName.PayPalOrderNumberIndex
            );
            services.AddKeyedScoped<IIndexProjectionRepository, UserCodeIndexRepository>(
                InternalProjectionName.UserCodeIndex
            );
            services.AddKeyedScoped<IIndexProjectionRepository, UserRefreshTokenIndexRepository>(
                InternalProjectionName.UserRefreshTokenIndex
            );
        }

        private static void AddAggregateRepositories(IServiceCollection services)
        {
            services.AddEventStoreRepository<Core.User.User>();
            services.AddEventStoreRepository<Core.Group.Group>();
            services.AddEventStoreRepository<Core.Expense.Expense>();
            services.AddEventStoreRepository<UserToken>();
        }

        private static void AddServices(IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<IPasswordHasher<Core.User.User>, PasswordHasher<Core.User.User>>();
            services.AddScoped<IAuthTokenService, AuthTokenService>();
        }

        private static void AddInfrastructure(IServiceCollection services, IConfiguration config)
        {
            services.AddPayPal(config);
        }
    }
}
