namespace Core.Group.Events
{
    public sealed record ExpenseRemovedFromGroup(Guid GroupId, Guid ExpenseId);
}
