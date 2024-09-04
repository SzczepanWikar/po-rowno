namespace Core.Common.PayPal.DTO
{
    public sealed record CreateOrderDto(
        string intent,
        IReadOnlyList<PurchaseUnitDto> purchase_units
    );

    public sealed record PurchaseUnitDto(AmountDto amount);

    public sealed record AmountDto(string currency_code, decimal value);
}
