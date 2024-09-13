namespace Core.User.Events
{
    public sealed record AccountDeleted(Guid Id, string Email);
}
