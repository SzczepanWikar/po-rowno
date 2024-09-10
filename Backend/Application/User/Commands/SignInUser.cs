using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.User.Services;
using Core.Common.Configs;
using Core.Common.Exceptions;
using Core.Common.Projections;
using Core.User;
using Core.User.Events;
using Infrastructure.Projections.InternalProjections.Repository;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.User.Commands
{
    using User = Core.User.User;

    public record SignInUser(string Email, string Password) : IRequest<AppSignInResult>;

    public class SignInUserHandler : IRequestHandler<SignInUser, AppSignInResult>
    {
        private readonly IUserService _userService;
        private readonly IIndexProjectionRepository _indexedEmailRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IAuthTokenService _authTokenService;

        public SignInUserHandler(
            [FromKeyedServices(InternalProjectionName.EmailIndex)]
                IIndexProjectionRepository emailConflictValidator,
            IUserService userService,
            IPasswordHasher<User> passwordHasher,
            IAuthTokenService authTokenService
        )
        {
            _indexedEmailRepository = emailConflictValidator;
            _userService = userService;
            _passwordHasher = passwordHasher;
            _authTokenService = authTokenService;
        }

        public async Task<AppSignInResult> Handle(
            SignInUser request,
            CancellationToken cancellationToken
        )
        {
            var user = await GetUser(request, cancellationToken);

            ValidateAuth(request, user);

            var accessToken = _authTokenService.GenerateAccessToken(user);
            var refreshToken = _authTokenService.GenerateRefreshToken();

            var res = new AppSignInResult(user.Id, user!.Status, accessToken, refreshToken.Token);

            var @event = new UserSignedIn(user.Id, accessToken, refreshToken);
            await _userService.AppendAsync(user.Id, @event, cancellationToken);

            return res;
        }

        private async Task<User> GetUser(SignInUser request, CancellationToken cancellationToken)
        {
            var userId = await _indexedEmailRepository.GetOwnerId(request.Email, cancellationToken);

            if (!userId.HasValue)
            {
                throw new BadRequestException("Incorrect credentials.");
            }

            var user = await _userService.FindOneAsync(userId.Value, cancellationToken);

            return user;
        }

        private void ValidateAuth(SignInUser request, User user)
        {
            if (user.Status != UserStatus.Active)
            {
                throw new BadRequestException("User is not active.");
            }

            var passwordVerificationRes = _passwordHasher.VerifyHashedPassword(
                user,
                user.Password,
                request.Password
            );

            if (passwordVerificationRes == PasswordVerificationResult.Failed)
            {
                throw new BadRequestException("Incorrect credentials.");
            }
        }
    }
}
