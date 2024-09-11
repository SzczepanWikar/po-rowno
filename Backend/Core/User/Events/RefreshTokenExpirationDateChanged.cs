namespace Core.User.Events
{
    public sealed record RefreshTokenExpirationDateChanged(
        Guid Id,
        string Token,
        DateTime ExpirationDate
    );
}
