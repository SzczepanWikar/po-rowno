namespace Core.Common.Configs
{
    public sealed record JwtConfig(
        string Token,
        uint ExpiresInSeconds,
        string Issuer,
        string Audience
    );
}
