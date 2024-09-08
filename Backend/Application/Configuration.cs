using Application.Expense;
using Application.Group;
using Application.User;
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
            services.AddEventStoreRepository<Core.Foo.Foo>();
            services.AddEventStoreRepository<Core.User.User>();
            services.AddEventStoreRepository<Core.Group.Group>();
            services.AddEventStoreRepository<Core.Expense.Expense>();
            services.AddEventStoreRepository<UserToken>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<IPasswordHasher<Core.User.User>, PasswordHasher<Core.User.User>>();

            services.AddKeyedScoped<IIndexProjectionRepository, GroupIndexRepository>(
                InternalProjectionName.GroupCodeIndex
            );
            services.AddKeyedScoped<IIndexProjectionRepository, UserEmailIndexRepository>(
                InternalProjectionName.EmailIndex
            );
            services.AddKeyedScoped<IIndexProjectionRepository, PaymentIndexRepository>(
                InternalProjectionName.PayPalOrderNumberIndex
            );
            services.AddKeyedScoped<IIndexProjectionRepository, UserIndexRepository>(
                InternalProjectionName.UserCodeIndex
            );

            services.AddPayPal(config);
            services.AddProjections<ApplicationContext>(config);

            return services;
        }

        public static WebApplication UseBusinessLogic(this WebApplication app) => app;
    }
}
