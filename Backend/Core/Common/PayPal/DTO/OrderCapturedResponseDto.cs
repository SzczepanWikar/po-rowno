namespace Core.Common.PayPal.DTO
{
    public sealed record OrderCapturedResponseDto(
        string id,
        string status,
        IReadOnlyList<PayPalLink> links
    );
}
