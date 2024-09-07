namespace Core.User.Events
{
    public sealed record UserPaswordChanged(Guid Id, string Password);
}
