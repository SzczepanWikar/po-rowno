namespace Core.Group.Events
{
    public sealed record UserUnbannedFromGroup(Guid GroupId, Guid userId);
}
