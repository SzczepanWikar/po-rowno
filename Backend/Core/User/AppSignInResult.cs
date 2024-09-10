namespace Core.User
{
    public record AppSignInResult(
        Guid Id,
        UserStatus Status,
        string AccessToken,
        string RefreshToken
    );
}
