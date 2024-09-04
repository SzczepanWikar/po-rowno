using Core.Common.PayPal.DTO;

namespace Core.Common.PayPal
{
    public sealed record CreatedOrder(OrderCreatedResponseDto Response, string OriginalResponse);
}
