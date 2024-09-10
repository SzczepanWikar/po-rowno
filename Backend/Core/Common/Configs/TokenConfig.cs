namespace Core.Common.Configs
{
    public sealed record TokenConfig(
        string Token,
        uint ExpiresInSeconds,
        string Issuer,
        string Audience,
        uint RefreshExpiresInSeconds
    );
}
