namespace Core.Common.PayPal
{
    public sealed record CapturedOrder(OrderCapturedResponse Response, string OriginalResponse);
}
