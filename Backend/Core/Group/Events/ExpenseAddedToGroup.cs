namespace Core.Group.Events
{
    public sealed record ExpenseAddedToGroup(Guid GroupId, Guid ExpenseId);
}
