namespace Core.UserGroupEvents
{
    public sealed record UserUnbannedFromGroup(Guid GroupId, Guid UserId);
}
