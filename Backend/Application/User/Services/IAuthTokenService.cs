using Core.User;
using Core.User.Events;

namespace Application.User.Services
{
    public interface IAuthTokenService
    {
        string GenerateAccessToken(Core.User.User user);
        RefreshToken GenerateRefreshToken();
        Task<IEnumerable<RefreshTokenExpirationDateChanged>> BlackListRefreshTokens(
            Guid id,
            CancellationToken cancellationToken = default
        );
    }
}
