using System.Text;
using System.Text.Json;
using Core.Common.Configs;
using Core.Common.PayPal;
using Core.Group;
using Microsoft.Extensions.Logging;

namespace Infrastructure.PayPal
{
    public class PayPalService : IPayPalService
    {
        private readonly PayPalConfig _payPalConfig;
        private readonly WebConfig _webConfig;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<PayPalService> _logger;

        public PayPalService(
            PayPalConfig payPalConfig,
            WebConfig webConfig,
            IHttpClientFactory httpClientFactory
        )
        {
            _payPalConfig = payPalConfig;
            _httpClientFactory = httpClientFactory;
            _webConfig = webConfig;
        }

        public async Task<CreatedOrder> Create(NewOrder newOrder)
        {
            var accessToken = await GetAccessToken();
            var payload =
                $@" {{
                    ""intent"": ""CAPTURE"",
                    ""purchase_units"": [
                        {{
                            ""amount"": {{
                                ""currency_code"": ""{CurrencyToCode(newOrder.Currency)}"",
                                ""value"": ""{newOrder.Amount.ToString().Replace(',', '.')}""
                            }},
                            ""payee"": {{
                                ""email_address"": ""{newOrder.PayeeEmail}""
                            }},
                            ""description"": ""{newOrder.Description}""
                        }}
                    ],
                    ""application_context"": {{
                        ""return_url"": ""{_webConfig.BaseUrl}/payment/{newOrder.ExpenseId}/success"",
                        ""cancel_url"": ""{_webConfig.BaseUrl}/payment/{newOrder.ExpenseId}/cancel""
                    }}
                }}";

            var requestContent = new StringContent(payload, Encoding.UTF8, "application/json");

            var httpClient = _httpClientFactory.CreateClient("PayPal");

            var request = new HttpRequestMessage(HttpMethod.Post, "/v2/checkout/orders")
            {
                Content = requestContent,
            };

            request.Headers.Add("Authorization", $"Bearer {accessToken}");

            var response = await httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var responseObject = JsonSerializer.Deserialize<OrderCreatedResponse>(
                    responseContent
                );
                var res = new CreatedOrder(responseObject, responseContent);

                return res;
            }
            else
            {
                throw new HttpRequestException(await response.Content.ReadAsStringAsync());
            }
        }

        public async Task<CapturedOrder> Capture(string orderId)
        {
            var accessToken = await GetAccessToken();

            var httpClient = _httpClientFactory.CreateClient("PayPal");

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"/v2/checkout/orders/{orderId}/capture"
            );
            request.Headers.Add("Authorization", $"Bearer {accessToken}");
            request.Content = new StringContent(string.Empty, Encoding.UTF8, "application/json");

            var response = await httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var responseObject = JsonSerializer.Deserialize<OrderCapturedResponse>(
                    responseContent
                );
                var res = new CapturedOrder(responseObject, responseContent);

                return res;
            }
            else
            {
                throw new HttpRequestException(await response.Content.ReadAsStringAsync());
            }
        }

        private async Task<string> GetAccessToken()
        {
            var auth = Convert.ToBase64String(
                Encoding.ASCII.GetBytes($"{_payPalConfig.ClientId}:{_payPalConfig.ClientSecret}")
            );

            var httpClient = _httpClientFactory.CreateClient("PayPal");

            var request = new HttpRequestMessage(HttpMethod.Post, "/v1/oauth2/token");

            request.Content = new StringContent(
                "grant_type=client_credentials",
                Encoding.UTF8,
                "application/x-www-form-urlencoded"
            );
            request.Headers.Add("Authorization", $"Basic {auth}");

            var response = await httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var text = await response.Content.ReadAsStringAsync();
                var obj = JsonSerializer.Deserialize<PayPalAuthResult>(text);

                return obj.access_token;
            }
            else
            {
                throw new HttpRequestException("Cannot log in to PayPal.");
            }
        }

        private string CurrencyToCode(Currency currency) =>
            currency switch
            {
                Currency.Dollar => "USD",
                Currency.Euro => "EUR",
                Currency.PolishZloty => "PLN",
                _ => throw new NotImplementedException("Currency not supported"),
            };

        internal sealed record PayPalAuthResult(
            string scope,
            string access_token,
            string token_type,
            string app_id,
            int expires_in,
            string nonce
        );
    }
}
