using Core.Common.Projections;
using Infrastructure.Projections.InternalProjections;
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

            var projections = new (string Name, string FileName)[]
            {
                (Name: InternalProjectionName.EmailIndex, FileName: "EmailIndex"),
                (Name: InternalProjectionName.GroupCodeIndex, "GroupCodeIndex"),
                (Name: InternalProjectionName.PayPalOrderNumberIndex, "PayPalOrderNumberIndex"),
                (Name: InternalProjectionName.UserCodeIndex, "UserCodeIndex")
            };

            services.AddKeyedSingleton<IReadOnlyCollection<(string Name, string FileName)>>(
                "InternalProjections",
                projections
            );

            services.AddHostedService<ReadModelProjector>();
            services.AddHostedService<InternalProjectionInitializer>();

            return services;
        }

        public static WebApplication UseProjections(this WebApplication app) => app;
    }
}
