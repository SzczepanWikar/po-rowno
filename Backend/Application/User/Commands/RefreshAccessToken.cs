using Application.User.Services;
using Core.Common.Exceptions;
using Core.Common.Projections;
using Core.User;
using Infrastructure.Projections.InternalProjections.Repository;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application.User.Commands
{
    using User = Core.User.User;

    public sealed record RefreshAccessToken(string RefreshToken) : IRequest<AppSignInResult>;

    public sealed class RefreshAccessTokenHandler
        : IRequestHandler<RefreshAccessToken, AppSignInResult>
    {
        private readonly IIndexProjectionRepository _indexProjectionRepository;
        private readonly IUserService _userService;
        private readonly IAuthTokenService _authTokenService;

        public RefreshAccessTokenHandler(
            [FromKeyedServices(InternalProjectionName.UserRefreshTokenIndex)]
                IIndexProjectionRepository indexProjectionRepository,
            IUserService userService,
            IAuthTokenService authTokenService
        )
        {
            _indexProjectionRepository = indexProjectionRepository;
            _userService = userService;
            _authTokenService = authTokenService;
        }

        public async Task<AppSignInResult> Handle(
            RefreshAccessToken request,
            CancellationToken cancellationToken
        )
        {
            var user = await GetUser(request, cancellationToken);
            var refreshToken = GetRefreshToken(user);

            var authToken = _authTokenService.GenerateAccessToken(user);

            var res = new AppSignInResult(user.Id, user.Status, authToken, refreshToken.Token);

            return res;
        }

        private async Task<User> GetUser(
            RefreshAccessToken request,
            CancellationToken cancellationToken
        )
        {
            var userId = await _indexProjectionRepository.GetOwnerId(request.RefreshToken);

            if (!userId.HasValue)
            {
                throw new UnauthorizedException();
            }

            try
            {
                var user = await _userService.FindOneAsync(userId.Value, cancellationToken);
                return user;
            }
            catch (NotFoundException ex)
            {
                throw new UnauthorizedException();
            }
        }

        private static RefreshToken GetRefreshToken(User user)
        {
            var refreshToken = user.RefreshTokens.LastOrDefault();

            if (refreshToken is null)
            {
                throw new UnauthorizedException();
            }

            if (refreshToken.ExpirationDate < DateTime.Now)
            {
                throw new UnauthorizedException();
            }

            return refreshToken;
        }
    }
}
