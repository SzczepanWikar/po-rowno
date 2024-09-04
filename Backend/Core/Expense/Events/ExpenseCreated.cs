using Core.Common.PayPal;
using Core.Group;

namespace Core.Expense.Events
{
    public sealed record ExpenseCreated(
        Guid Id,
        string Name,
        decimal Amount,
        Currency Currency,
        ExpenseType Type,
        Guid GroupId,
        Guid PayerId,
        IReadOnlyList<Guid> DeptorsIds,
        CreatedOrder? Payment = null
    );
}
