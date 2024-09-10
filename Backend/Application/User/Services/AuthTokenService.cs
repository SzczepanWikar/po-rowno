using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Core.Common.Configs;
using Core.User;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Application.User.Services
{
    using User = Core.User.User;

    public class AuthTokenService : IAuthTokenService
    {
        private readonly TokenConfig _tokenConfig;

        public AuthTokenService(IConfiguration configuration)
        {
            var tokenConfig = configuration.GetSection("Token").Get<TokenConfig>();

            if (tokenConfig == null)
            {
                throw new InvalidOperationException("JWT configuration is missing");
            }

            _tokenConfig = tokenConfig;
        }

        public string GenerateAccessToken(User user)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenConfig.Token));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddSeconds(_tokenConfig.ExpiresInSeconds),
                signingCredentials: signingCredentials,
                issuer: _tokenConfig.Issuer,
                audience: _tokenConfig.Audience
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        public RefreshToken GenerateRefreshToken()
        {
            var randomNumber = new byte[64];

            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            var token = Convert.ToBase64String(randomNumber);
            var expirationDate = DateTime.Now.AddSeconds(_tokenConfig.RefreshExpiresInSeconds);

            return new RefreshToken(token, expirationDate);
        }
    }
}
