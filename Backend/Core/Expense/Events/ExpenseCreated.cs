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
        IList<Guid> DeptorsIds
    );
}
