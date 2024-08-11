using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.User.Auth;
using Core.Common.Configs;
using Core.Common.Exceptions;
using Core.User;
using Infrastructure.EventStore.Repository;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Application.User.Commands
{
    using User = Core.User.User;

    public record SignInUser(string Email, string Password) : IRequest<AppSignInResult>;

    public class SignInUserHandler : IRequestHandler<SignInUser, AppSignInResult>
    {
        private readonly IEventStoreRepository<User> _eventStoreRepository;
        private readonly EmailConflictValidator _emailConflictValidator;
        private readonly IConfiguration _configuration;

        public SignInUserHandler(
            IEventStoreRepository<User> eventStoreRepository,
            EmailConflictValidator emailConflictValidator,
            IConfiguration configuration
        )
        {
            _eventStoreRepository = eventStoreRepository;
            _emailConflictValidator = emailConflictValidator;
            _configuration = configuration;
        }

        public async Task<AppSignInResult> Handle(
            SignInUser request,
            CancellationToken cancellationToken
        )
        {
            var userId = await _emailConflictValidator.GetUserId(request.Email, cancellationToken);

            if (!userId.HasValue)
            {
                throw new BadRequestException("Incorrect credentials.");
            }

            var userIdVal = userId.Value;
            var user = await _eventStoreRepository.Find(userIdVal, cancellationToken);

            ValidateAuth(request, user);

            var token = GenerateToken(user!);

            var res = new AppSignInResult(userIdVal, user!.Status, token);

            return res;
        }

        private static void ValidateAuth(SignInUser request, User? user)
        {
            if (user == null)
            {
                throw new BadRequestException("Incorrect credentials.");
            }

            if (user.Status != UserStatus.Active)
            {
                throw new BadRequestException("User is not active.");
            }

            if (!BCrypt.Net.BCrypt.EnhancedVerify(request.Password, user.Password))
            {
                throw new BadRequestException("Incorrect credentials.");
            }
        }

        private string GenerateToken(User user)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var jwtConfig = _configuration.GetSection("JWT").Get<JwtConfig>();

            if (jwtConfig == null)
            {
                throw new InvalidOperationException("JWT configuration is missing");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Token));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddSeconds(jwtConfig.ExpiresInSeconds),
                signingCredentials: signingCredentials,
                issuer: jwtConfig.Issuer,
                audience: jwtConfig.Audience
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}
