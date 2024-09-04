namespace Infrastructure.PayPal
{
    public sealed record PayPalConfig(string ClientId, string ClientSecret, string ApiAddress);
}
