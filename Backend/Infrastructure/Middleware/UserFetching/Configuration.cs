using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Middleware.UserFetching
{
    public static class Configuration
    {
        public static IServiceCollection AddUserFetchng(this IServiceCollection services)
        {
            services.AddScoped<UserFetcher>();
            services.AddScoped<UserFetchingMiddleware>();

            return services;
        }

        public static WebApplication UseUserFetching(this WebApplication app)
        {
            app.UseMiddleware<UserFetchingMiddleware>();

            return app;
        }
    }
}
