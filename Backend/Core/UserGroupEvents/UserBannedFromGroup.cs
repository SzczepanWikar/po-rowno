namespace Core.UserGroupEvents
{
    public sealed record UserBannedFromGroup(Guid GroupId, Guid UserId);
}
