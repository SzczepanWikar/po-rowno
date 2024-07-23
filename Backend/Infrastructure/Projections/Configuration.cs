using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Projections
{
    public static class Configuration
    {
        public static IServiceCollection AddProjections<T>(
            this IServiceCollection services,
            IConfiguration config
        )
            where T : DbContext
        {
            services.AddDbContext<T>(options =>
            {
                options.UseSqlServer(config.GetConnectionString("ReadModel"));
            });
            services.AddHostedService<ReadModelProjector>();

            return services;
        }

        public static WebApplication UseProjections(this WebApplication app) => app;
    }
}
