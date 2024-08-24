namespace Core.Group.Events
{
    public sealed record UserBannedFromGroup(Guid GroupId, Guid userId);
}
