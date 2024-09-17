using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Core.Common.Configs;
using Core.User;
using Core.User.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Application.User.Services
{
    using User = Core.User.User;

    public class AuthTokenService : IAuthTokenService
    {
        private readonly TokenConfig _tokenConfig;
        private readonly IUserService _userService;

        public AuthTokenService(IConfiguration configuration, IUserService userService)
        {
            var tokenConfig = configuration.GetSection("Token").Get<TokenConfig>();

            if (tokenConfig == null)
            {
                throw new InvalidOperationException("JWT configuration is missing");
            }

            _tokenConfig = tokenConfig;
            _userService = userService;
        }

        public string GenerateAccessToken(User user)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
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

        public async Task<IEnumerable<RefreshTokenExpirationDateChanged>> BlackListRefreshTokens(
            Guid id,
            CancellationToken cancellationToken = default
        )
        {
            var user = await _userService.FindOneAsync(id, cancellationToken);

            var now = DateTime.Now;
            var events = user
                .RefreshTokens.Where(e => e.ExpirationDate > now)
                .Select(e => new RefreshTokenExpirationDateChanged(user.Id, e.Token, now))
                .ToList();

            await _userService.AppendAsync(user.Id, events, cancellationToken);

            return events;
        }
    }
}
