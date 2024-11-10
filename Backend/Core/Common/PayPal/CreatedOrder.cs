namespace Core.Common.PayPal
{
    public sealed record CreatedOrder(OrderCreatedResponse Response, string OriginalResponse);
}
