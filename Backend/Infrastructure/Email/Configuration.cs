using Infrastructure.Email.Service;
using Infrastructure.Email.Store;
using Infrastructure.EventStore.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Email
{
    public static class Configuration
    {
        public static IServiceCollection AddMailing(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            var emailConfig = configuration.GetSection("EmailConfiguration").Get<EmailConfig>();

            if (emailConfig == null)
            {
                throw new Exception("EmailConfiguration is not provided");
            }

            services.AddSingleton(emailConfig);
            services.AddScoped<IEmailService, EmailService>();
            services.AddEventStoreRepository<EmailMessageAggregate>();

            return services;
        }
    }
}
