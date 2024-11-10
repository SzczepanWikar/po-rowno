namespace Core.Common.PayPal
{
    public sealed record OrderCapturedResponse(
        string id,
        string status,
        IReadOnlyList<PayPalLink> links
    );
}
