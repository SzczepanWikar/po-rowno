using Core.Common.PayPal;

namespace Core.Expense.Events
{
    public sealed record ExpensePaymentCaptured(Guid Id, CapturedOrder CapturedOrder);
}
