using Core.Group;

namespace Core.Common.PayPal
{
    public sealed record NewOrder(
        decimal Amount,
        Currency Currency,
        string PayeeEmail,
        Guid ExpenseId,
        string Description
    );
}
