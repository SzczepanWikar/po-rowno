namespace Core.User
{
    public sealed record RefreshToken(string Token, DateTime ExpirationDate);
}
