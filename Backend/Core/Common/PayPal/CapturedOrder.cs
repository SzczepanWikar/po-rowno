using Core.Common.PayPal.DTO;

namespace Core.Common.PayPal
{
    public sealed record CapturedOrder(OrderCapturedResponseDto Response, string OriginalResponse);
}
