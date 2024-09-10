using System.Text;
using Core.Common.Configs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Middleware.Auth
{
    public static class Configuration
    {
        public static IServiceCollection AddAuth(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            var tokenConfig = configuration.GetSection("Token").Get<TokenConfig>();

            if (tokenConfig is null)
            {
                throw new InvalidOperationException("JWT configuration is missing");
            }

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(tokenConfig.Token)
                        ),
                        ValidIssuer = tokenConfig.Issuer,
                        ValidAudience = tokenConfig.Audience,
                    };
                });

            return services;
        }

        public static WebApplication UseAuth(this WebApplication app)
        {
            app.UseAuthentication();
            app.UseAuthorization();

            return app;
        }
    }
}
