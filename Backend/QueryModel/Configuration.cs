using Infrastructure.Projections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QueryModel.Expense;

namespace QueryModel
{
    public static class Configuration
    {
        public static IServiceCollection AddDatabaseProjections(
            this IServiceCollection services,
            IConfiguration config
        )
        {
            services.AddScoped<BalanceCalculator>();
            services.AddProjections<ApplicationContext>(config);
            return services;
        }
    }
}
