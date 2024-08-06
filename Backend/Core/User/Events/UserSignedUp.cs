namespace Core.User.Events
{
    public record UserSignedUp(Guid Id, string Username, string Email, string Password);
}
