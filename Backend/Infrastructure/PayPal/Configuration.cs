using Core.Common.Configs;
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

            var url = config.GetValue<string>("BaseUrl");

            if (url is null) {
                throw new InvalidOperationException("BaseUrl configuration is missing");
            }

            WebConfig webConfig = new(url);

            services.AddSingleton<PayPalConfig>(payPalConfig);
            services.AddSingleton<WebConfig>(webConfig);
            
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
