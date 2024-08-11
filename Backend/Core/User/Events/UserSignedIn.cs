namespace Core.User.Events
{
    public sealed record UserSignedIn(Guid Id, string Token);
}
