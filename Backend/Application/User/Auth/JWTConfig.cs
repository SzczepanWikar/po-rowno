namespace Application.User.Auth
{
    public sealed record JWTConfig(string Token, uint ExpiresInSeconds);
}
