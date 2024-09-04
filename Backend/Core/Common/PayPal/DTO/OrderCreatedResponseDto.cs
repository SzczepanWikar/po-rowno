namespace Core.Common.PayPal.DTO
{
    public record OrderCreatedResponseDto(
        string id,
        string status,
        IReadOnlyList<PayPalLink> links
    );
}
