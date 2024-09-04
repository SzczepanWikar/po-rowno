using System.Text;
using System.Text.Json;
using Core.Common.PayPal;
using Core.Common.PayPal.DTO;
using Core.Group;

namespace Infrastructure.PayPal
{
    public class PayPalService : IPayPalService
    {
        private readonly PayPalConfig _payPalConfig;
        private readonly IHttpClientFactory _httpClientFactory;

        public PayPalService(PayPalConfig payPalConfig, IHttpClientFactory httpClientFactory)
        {
            _payPalConfig = payPalConfig;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<CreatedOrder> Create(Currency currency, decimal amount)
        {
            var accessToken = await GetAccessToken();

            var purchaseUnits = new List<PurchaseUnitDto>
            {
                new PurchaseUnitDto(new AmountDto(CurrencyToCode(currency), amount))
            };

            var dto = new CreateOrderDto("CAPTURE", purchaseUnits);
            var jsonPayload = JsonSerializer.Serialize(dto);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var httpClient = _httpClientFactory.CreateClient("PayPal");

            var request = new HttpRequestMessage(HttpMethod.Post, "/v2/checkout/orders")
            {
                Content = content
            };

            request.Headers.Add("Authorization", $"Bearer {accessToken}");

            var response = await httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var responseObject = JsonSerializer.Deserialize<OrderCreatedResponseDto>(
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
                var responseObject = JsonSerializer.Deserialize<OrderCapturedResponseDto>(
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
                _ => throw new NotImplementedException("Currency not supported")
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
