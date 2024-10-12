namespace Core.Expense.Events
{
    public sealed record Deptor(Guid UserId, decimal Amount);
}
