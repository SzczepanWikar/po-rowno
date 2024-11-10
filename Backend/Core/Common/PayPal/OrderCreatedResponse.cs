namespace Core.Common.PayPal
{
    public record OrderCreatedResponse(string id, string status, IReadOnlyList<PayPalLink> links);
}
