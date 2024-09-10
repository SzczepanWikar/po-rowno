using Core.User;

namespace Application.User.Services
{
    public interface IAuthTokenService
    {
        string GenerateAccessToken(Core.User.User user);
        RefreshToken GenerateRefreshToken();
    }
}
