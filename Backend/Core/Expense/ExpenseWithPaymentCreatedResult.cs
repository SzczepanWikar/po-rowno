using Core.Common.PayPal;

namespace Core.Expense
{
    public sealed record ExpenseWithPaymentCreatedResult(
        Guid Id,
        string OrderId,
        IEnumerable<PayPalLink> links
    );
}
