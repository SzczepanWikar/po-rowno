using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.PayPal
{
    public static class Configuration
    {
        public static IServiceCollection AddPayPal(
            this IServiceCollection services,
            IConfiguration config
        )
        {
            var payPalConfig = config.GetSection("PayPal").Get<PayPalConfig>();

            if (payPalConfig is null)
            {
                throw new InvalidOperationException("PayPal configuration is missing");
            }

            services.AddSingleton<PayPalConfig>(payPalConfig);
            services.AddHttpClient(
                "PayPal",
                httpClient =>
                {
                    httpClient.BaseAddress = new Uri(payPalConfig.ApiAddress);
                }
            );
            services.AddScoped<IPayPalService, PayPalService>();

            return services;
        }
    }
}
