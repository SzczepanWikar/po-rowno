namespace Core.User
{
    public record AppSignInResult(Guid Id, UserStatus status, string Token);
}
