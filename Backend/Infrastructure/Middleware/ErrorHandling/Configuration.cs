using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Middleware.ErrorHandling
{
    public static class Configuration
    {
        public static IServiceCollection AddHttpExceptionHandlingMiddleware(
            this IServiceCollection services
        )
        {
            services.AddScoped<HttpExceptionHandlingMiddleware>();
            return services;
        }

        public static WebApplication UseHttpExceptionHandlingMiddleware(this WebApplication app)
        {
            app.UseMiddleware<HttpExceptionHandlingMiddleware>();

            return app;
        }
    }
}
